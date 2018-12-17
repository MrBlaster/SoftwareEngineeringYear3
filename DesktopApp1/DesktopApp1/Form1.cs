using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using System.Data.Sql;
using System.Data.SqlClient;

// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace DesktopApp1
{
	public partial class Form1 : Form
	{
		static string baseAddress = "http://www.omdbapi.com/?i=tt3896198&apikey=edefce46";

		public Form1()
		{
			InitializeComponent();
		}

		public class Movie
		{
			public string movieName;
			public string movieYear;
			public string movieRating;
			public string movieDuration;
			public string movieReleased;
			public string[] movieGenre = new string[1];
			public string[] movieDirector = new string[1];
			public string[] movieWriter = new string[1];
			public string[] movieActors = new string[1];
			public string moviePlot;
			public string moviePoster;
			public string movieCountry;
			public decimal imdbRating;

			public string imdbID;

		}

		public Movie currentMovie = null;
		private HttpClient client;

		public class ApiResponse
		{
			public string initReturn;

			public bool returnTrue;
		}

		public string ProduceAttribute(int characterPosition, string initReturn)
		{
			bool isAttribute = false;

			string attribute = "";

			isAttribute = false;

			for (int i = characterPosition + 3; i < initReturn.Length; i++)
			{
				if ((isAttribute == true) && (initReturn[i] != '\"'))
				{
					attribute = attribute + initReturn[i];
				}

				if (initReturn[i] == '\"')
				{
					if (isAttribute == false)
					{
						isAttribute = true;
					}
					else
					{
						//characterPosition = i;
						break;
					}
				}
			}

			return attribute;
		}


		public Movie GenerateMovie(string apiResponse)
		{
			Movie movie = new Movie();

			string plainText = apiResponse;

			string attribute = "";

			for (int characterPosition = 1; characterPosition < plainText.Length; characterPosition++)
			{
				char c = plainText[characterPosition];

				if ((c != '{') && (c != '}') && (c != '"') && (c != ':') && (c != ','))
				{
					attribute = attribute + c;
				}

				if (c == '\"')
				{
					attribute = "";
				}

				switch (attribute)
				{
					case "Title":

						attribute = ProduceAttribute(characterPosition, plainText);

						movie.movieName = attribute;

						characterPosition = characterPosition + attribute.Length + 3;

						attribute = "";

						break;

					case "Year":

						attribute = ProduceAttribute(characterPosition, plainText);

						movie.movieYear = attribute;

						characterPosition = characterPosition + attribute.Length + 3;

						attribute = "";

						break;

					case "Rated":

						attribute = ProduceAttribute(characterPosition, plainText);

						movie.movieRating = attribute;

						characterPosition = characterPosition + attribute.Length + 3;

						attribute = "";

						break;

					case "Runtime":

						attribute = ProduceAttribute(characterPosition, plainText);

						movie.movieDuration = Convert.ToString(attribute);

						characterPosition = characterPosition + attribute.Length + 3;

						attribute = "";

						break;

					case "Released":

						attribute = ProduceAttribute(characterPosition, plainText);

						movie.movieReleased = Convert.ToString(attribute);

						characterPosition = characterPosition + attribute.Length + 3;

						attribute = "";

						break;

					case "Genre":

						attribute = ProduceAttribute(characterPosition, plainText);

						movie.movieGenre[0] = attribute;

						characterPosition = characterPosition + attribute.Length + 5;

						attribute = "";

						break;

					case "Director":

						attribute = ProduceAttribute(characterPosition, plainText);

						movie.movieDirector[0] = attribute;

						characterPosition = characterPosition + attribute.Length + 3;

						attribute = "";

						break;

					case "Writer":

						attribute = ProduceAttribute(characterPosition, plainText);

						movie.movieWriter[0] = attribute;

						characterPosition = characterPosition + attribute.Length + 5;

						attribute = "";

						break;

					case "Actors":

						attribute = ProduceAttribute(characterPosition, plainText);

						movie.movieActors[0] = attribute;

						characterPosition = characterPosition + attribute.Length + 6;

						attribute = "";

						break;

					case "Plot":

						attribute = ProduceAttribute(characterPosition, plainText);

						movie.moviePlot = attribute;

						characterPosition = characterPosition + attribute.Length + 5;

						attribute = "";

						break;

					case "imdbID":

						attribute = ProduceAttribute(characterPosition, plainText);

						movie.imdbID = attribute;

						characterPosition = characterPosition + attribute.Length + 5;

						attribute = "";

						break;

					case "Poster":

						attribute = ProduceAttribute(characterPosition, plainText);

						movie.moviePoster = attribute;

						characterPosition = characterPosition + attribute.Length + 5;

						attribute = "";

						break;

					case "Country":

						attribute = ProduceAttribute(characterPosition, plainText);

						movie.movieCountry = attribute;

						characterPosition = characterPosition + attribute.Length + 5;

						attribute = "";

						break;

					case "imdbRating":

						attribute = ProduceAttribute(characterPosition, plainText);

						if (attribute != "N/A")
						{
							movie.imdbRating = Convert.ToDecimal(attribute);
						}

						characterPosition = characterPosition + attribute.Length + 5;

						attribute = "";

						break;

				}
			}

			return movie;
		}

		private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			// Click on the link below to continue learning how to build a desktop app using WinForms!
			System.Diagnostics.Process.Start("http://aka.ms/dotnet-get-started-desktop");

		}

		private void button1_click_1(object sender, EventArgs e)
		{
			if (textBox1.Text != "")
			{

				client = new HttpClient();

				string x = RunAsync(baseAddress, "").GetAwaiter().GetResult();

				currentMovie = GenerateMovie(x);

				DisplayMovieInfo(currentMovie);

			}
		}

		public async Task<string> RunAsync(string baseAddress, string imdbID)
		{
			Uri uri = new Uri(baseAddress + "&t=" + textBox1.Text);

			if (imdbID != "")
			{
				uri = new Uri(baseAddress + "&i=" + imdbID);
			}

			try
			{
				HttpResponseMessage response = await client.GetAsync(uri).ConfigureAwait(false);
				response.EnsureSuccessStatusCode();
				string responseBody = await response.Content.ReadAsStringAsync();

				client.Dispose();

				return responseBody;
			}
			catch (HttpRequestException e)
			{
				Console.WriteLine("\nException Caught!");
				Console.WriteLine("Message :{0} ", e.Message);

				client.Dispose();

				return "Error: " + e.Message;
			}
		}

		private void DisplayMovieInfo(Movie currentMovie)
		{

			label2.Text = currentMovie.movieName + " (" + currentMovie.movieYear + ")";
			label3.Text = currentMovie.movieRating + " | " + currentMovie.movieDuration + " | " + currentMovie.movieReleased + " | " + currentMovie.movieCountry;
			label4.Text = currentMovie.movieGenre[0];
			label5.Text = currentMovie.movieDirector[0];
			label6.Text = currentMovie.movieActors[0];
			label7.Text = Convert.ToString(currentMovie.imdbRating) + "\u2605";
			label8.Text = currentMovie.moviePlot;

			if (currentMovie.moviePoster != null)
			{
				if (currentMovie.moviePoster == "N/A")
				{
					pictureBox1.Image = pictureBox1.ErrorImage;
				}
				else
				{
					var request = WebRequest.Create(currentMovie.moviePoster);
					try
					{
						using (var response = request.GetResponse())
						using (var stream = response.GetResponseStream())
						{
							pictureBox1.Image = Bitmap.FromStream(stream);
						}
					}
					catch (Exception ex)
					{
						pictureBox1.Image = pictureBox1.ErrorImage;
					}

				}
			}
			else
			{
				pictureBox1.Image = null;
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (textBox1.Text != "")
			{

				client = new HttpClient();

				string x = RunAsync(baseAddress, "").GetAwaiter().GetResult();

				currentMovie = GenerateMovie(x);

				DisplayMovieInfo(currentMovie);

			}
		}

		private void pictureBox1_Click(object sender, EventArgs e)
		{

		}

		private void label2_Click(object sender, EventArgs e)
		{

		}

		private void label7_Click(object sender, EventArgs e)
		{

		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void label15_Click(object sender, EventArgs e)
		{

		}

		private void label16_Click(object sender, EventArgs e)
		{

		}

		private void label6_Click(object sender, EventArgs e)
		{

		}

		private void label8_Click(object sender, EventArgs e)
		{

		}

		private void label5_Click(object sender, EventArgs e)
		{

		}

		private void label4_Click(object sender, EventArgs e)
		{

		}

		private void label3_Click(object sender, EventArgs e)
		{

		}

	

		private void button4_Click(object sender, EventArgs e)
		{
		}
		
	}
}
