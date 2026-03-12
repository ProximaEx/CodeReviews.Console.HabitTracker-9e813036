using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Globalization;


/* ----- Project Reqs -----
 
You should handle all possible errors so that the application never crashes.

Follow the DRY Principle, and avoid code repetition.

Your project needs to contain a Read Me file where
you'll explain how your app works and tell a little
bit about your thought progress. What was hard? What
was easy? What have you learned?

----- Formatting Needs -----

What will make me enjoy using this app?
> entrance animation
> window formatting
> ability to choose what field to edit in edit screen
	(edit only a record's date)
	(edit only a record's quantity)
> ability to search by date
> restructure options to put 'edit' and 'delete' into 'view all' screen
> rename 'view all' to 'habit history'
> nuke all records button w/ confirmations
> easter egg

----- Challenges -----

Write unit tests?

Parameterized queries?

Allow for multiple habits in one table, let users create habits and specify the units they are tracked in.

Seed data when created for the first time. Several habits and inserting 100 records automatically
 */

namespace habitTracker.ProximaEx;

class HabitTrackerProgram
{
	const string connectionString = "Data Source=coffee-tracker.db";
	static readonly SqliteConnection connection = new(connectionString);

	static void Main(string[] args)
	{
		using (connection)
		{
			connection.Open();
			var tableCmd = connection.CreateCommand();
			tableCmd.CommandText =
				@"CREATE TABLE IF NOT EXISTS habit_table (
				Id INTEGER PRIMARY KEY AUTOINCREMENT,
				Habit TEXT,
				Date TEXT,
				Quantity INTEGER
				)";
			tableCmd.ExecuteNonQuery();
			connection.Close();
		}
		LoadingScreen();
		Menu();
	}

	static void LoadingScreen()
	{
		Console.Clear();
		Console.WriteLine("Coffee Tracker (TM)");
		Console.WriteLine("Loading...");
		//var loading = new Thread();
		//loading.Start();
		Thread.Sleep(1000);
	}

	static void Menu()
	{
		bool exit = false;
		while (!exit)
		{
			DrawMenu();
			Console.SetCursorPosition(0, 14);
			Console.Write("                                          \r");
			string? menuInput = Console.ReadLine();

			switch (menuInput)
			{
				case "exit":
					exit = true;
					break;
				case "1":
					Insert();
					break;
				case "2":
					ViewAll();
					Console.WriteLine("Press any key to exit");
					Console.ReadKey();
					break;
				case "3":
					Edit();
					break;
				case "4":
					Delete();
					break;
				default:
					Console.WriteLine("Please enter a valid option.");
					Thread.Sleep(1000);
					break;
			}
		}
	}

	static void DrawMenu()
	{
		Console.Clear();
		string[] menuStrings = {
				"\n",
				"Welcome to Coffee Tracker",
				"Drink more coffee\n\n",
				"Please select an option:\n",
				" 1 - Log coffee drinking",
				" 2 - View coffee drinking",
				" 3 - Edit a coffee record",
				" 4 - Delete a coffee record\n\n\n\n\n\n",
				"Type 'exit' to leave."};
		foreach (string s in menuStrings) { Console.WriteLine(s); }
	}

	static void Insert()
	{
		Console.Clear();
		string date = GetDateInput();
		int quantity = GetNumInput("Enter quantity");

		using (connection)
		{
			connection.Open();
			var tableCmd = connection.CreateCommand();
			tableCmd.CommandText =
				$@"INSERT INTO habit_table ( Date, Quantity ) VALUES ( '{date}', {quantity} )";
			tableCmd.ExecuteNonQuery();
			connection.Close();
		}
	}

	static void ViewAll()
	{
		Console.Clear();
		List<CoffeeRecord> queryRecords = new();

		using (connection)
		{
			connection.Open();
			var tableCmd = connection.CreateCommand();
			tableCmd.CommandText = 
				@"SELECT * FROM habit_table";
			var reader = tableCmd.ExecuteReader();

			if (reader.HasRows)
			{
				while (reader.Read())
				{
					queryRecords.Add( new CoffeeRecord(reader.GetInt32(0),reader.GetString(1),reader.GetInt32(2)) );
				}
			} else { Console.WriteLine("No data found"); }

			reader.Close();
			connection.Close();
		}
		Console.WriteLine("\n------------- Records -------------");
		foreach (CoffeeRecord row in queryRecords)
		{
			Console.WriteLine($"Id: {row.Id}\t{row.Date}\tQuantity: {row.Quantity}");
		}
		Console.WriteLine("-----------------------------------\n");
	}

	static void Edit()
	{
		Console.Clear();
		ViewAll();
		int id = GetNumInput("Enter Id for record you want to change");

		using (connection)
		{
			connection.Open();
			var scalarCmd = connection.CreateCommand();
			scalarCmd.CommandText = 
				$@"SELECT EXISTS (SELECT 1 FROM habit_table WHERE Id = {id})";
			int isMatch = Convert.ToInt32(scalarCmd.ExecuteScalar());

			if (isMatch == 0)
			{
				Console.WriteLine("No match was found");
				connection.Close();
				Edit();
			}
			string date = GetDateInput();
			int quantity = GetNumInput("Enter new quantity");

			var tableCmd = connection.CreateCommand();
			tableCmd.CommandText =
				@$"UPDATE habit_table SET Date = '{date}', Quantity = {quantity} WHERE Id = {id}";
			tableCmd.ExecuteNonQuery();
			connection.Close();
		}
	}

	static void Delete()
	{
		Console.Clear();
		ViewAll();
		int id = GetNumInput("Enter the Id of the record you want to delete");

		using (connection)
		{
			connection.Open();
			var tableCmd = connection.CreateCommand();
			tableCmd.CommandText = 
				$@"DELETE FROM habit_table WHERE Id = {id}";
			int deletedRows = tableCmd.ExecuteNonQuery();
			if (deletedRows == 0) { Console.WriteLine("Couldn't find that record"); }
			else { Console.WriteLine("Record deleted"); }
			connection.Close();
		}
	}

	public static int GetNumInput(string message)
	{
		while (true)
		{
			Console.WriteLine(message);
			string? input = Console.ReadLine();
			if (int.TryParse(input, out int result) && result > 0 && result < 100) { return result; }
			else { Console.WriteLine("Invalid quantity"); }
		}
	}

	public static string GetDateInput()
	{
		while (true)
		{
			Console.WriteLine("Enter date (formatted mm-dd-yy)");
			string? input = Console.ReadLine();
			if (DateTime.TryParseExact(input, "MM-dd-yy", new CultureInfo("en-US"), 0, out DateTime outyDate)) { return input; }
			else { Console.WriteLine("Invalid date"); }
		}
	}
}

public class CoffeeRecord(int idIn, string dateIn, int quantityIn)
{
	public int Id { get; } = idIn;
	public string Date { get; } = dateIn;
	public int Quantity { get; } = quantityIn;
}

