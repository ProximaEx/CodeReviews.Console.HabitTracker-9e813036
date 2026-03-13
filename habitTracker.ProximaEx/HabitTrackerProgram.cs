using System;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;


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
	const string connectionString = "Data Source=habit-log.db";
	static readonly SqliteConnection connection = new(connectionString);
	static int width;
	static int height;
	static List<string> habitTypes = [ "Coffee" ];
	static int currentHabitIndex = 1;
	static bool SelectAllAdded = false;
	//Add 'all' functionality for viewing all habits in db
	//list to store habit types
	//after creating table, call sql query to add any other habit types

	static void Main(string[] args)
	{
		SetWindow();
		CreateTablesIfNotExists();
		GetHabitTypes();
		//FlashTitle();
		//CoffeeAnimation();
		Menu();
	}
	static void FlashTitle()
	{
		string[] titleScreen =
		{
			"                 ___   ___               ",
			"   ____   ____  ||``\\ ||``\\  ___   ___   ",
			"  ||``\\| ||``|| ||__  ||__  ||`|| ||`||  ",
			"  ||     ||  || |!``  |!``  |!==' |!=='  ",
			"  ||__/| ||__|| ||    ||    ||__. ||__.  ",
			"   ````   ````  ``    ``     ````  ````  ",
			"",
			"",
			"  .                        ",
			" /  __.  __~  __~  __,  ,_ ",
			"/  /__/ /__/ /__/ /--` /  `",
			"        ,_/  ,_/  `-       "
		};
		int topY = height / 2 - titleScreen.Length / 2;
		Console.SetCursorPosition(0, topY);
		foreach (string line in titleScreen) { Console.WriteLine(CenterText(line)); }
		Thread.Sleep(1200);

		string[,] notJustCoffee = { { CenterText("=========================================") } };
		Animate(notJustCoffee, [0, topY + 3], 500);
		string[,] message = { { CenterText("h a b i t") } };
		Animate(message, [0, topY + 6], 1500);

		Console.Clear();
	}

	static void CoffeeAnimation()
	{	
		string[,] loadCells =
		{
			{
			"        \\ .         ",
			"       ' ) )        ",
			"        / / '       ",
			"    ._________.     ",
			"    ||       ==v\\\\,  ",
			"    ||        |  || ",
			"    ||        |  || ",
			"    ||       ==*/`  ",
			"     \\\\_____./      ",
			},
			{
			"       ' ) ;        ",
			"        / / '       ",
			"       (  \\         ",
			"    ._________.     ",
			"    ||       ==v\\\\,  ",
			"    ||        |  || ",
			"    ||        |  || ",
			"    ||       ==*/`  ",
			"     \\\\_____./      ",
			},
			{
			"        ./ `        ",
			"       ( (.         ",
			"       ,\\  \\        ",
			"    ._________.     ",
			"    ||       ==v\\\\,  ",
			"    ||        |  || ",
			"    ||        |  || ",
			"    ||       ==*/`  ",
			"     \\\\_____./      ",
			},
			{
			"       c.(          ",
			"        ;\\ \\        ",
			"       ' ) .)       ",
			"    ._________.     ",
			"    ||       ==v\\\\,  ",
			"    ||        |  || ",
			"    ||        |  || ",
			"    ||       ==*/`  ",
			"     \\\\_____./      ",
			},
		};
		int[] topLeftCoordsForCenteredAnimation =
			[ width / 2 - loadCells[0,0].Length / 2 , height / 2 - loadCells.GetLength(1) / 2 ];
		for (int i = 0; i < 3; i++) { Animate(loadCells, topLeftCoordsForCenteredAnimation, 250); }
	}

	static void Menu()
	{	
		if (!SelectAllAdded && habitTypes.Count >= 2) { habitTypes.Add("All"); SelectAllAdded = true; }
		string[] menuStrings = [
				"\n",
				"Welcome to Coffee, um I mean Habit Logger",
				"Track whatever...",
				"\n",
				"   +---------------+   ",
				"<  |               |  >",
				"   +---------------+   ",
				"\n",
				"Please select an option:",
				"",
				" 1 - Log habit occurance   ",
				" 2 - View history          ",
				" 3 - Edit a record         ",
				" 4 - Delete a record       ",
				" 5 - Add a new habit type  ",
				"\n\n\n",
				"Type 'exit' to leave."
		];
		bool exit = false;
		while (!exit)
		{
			Draw(menuStrings);
			Console.SetCursorPosition(width / 2 - 7, 7);
			Console.Write(CenterText(habitTypes[currentHabitIndex], 13));
			Console.SetCursorPosition(width / 2 - 1, 19);
			var keyInput = Console.ReadKey(false);
			if (keyInput.Key == ConsoleKey.LeftArrow || keyInput.Key == ConsoleKey.RightArrow)
			{
				HabitSelector(keyInput);
			}
			else if (keyInput.Key == ConsoleKey.Backspace)
			{
				Console.Write("\b");
			}
			else
			{
				char charInput = keyInput.KeyChar;
				string menuInput = char.ToString(charInput) + Console.ReadLine();

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
						Console.ReadKey();
						break;
					case "3":
						Edit();
						break;
					case "4":
						Delete();
						break;
					//case "5":
					//	AddType();
					//	break;
					default:
						Console.Write(CenterText("Please enter a valid option."));
						Thread.Sleep(500);
						break;
				}
			}
		}
	}

	static void Draw(string[] cell)
	{
		Console.Clear();
		foreach (string s in cell) { Console.WriteLine(CenterText(s)); }
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
				$@"INSERT INTO habit_table ( Date, Quantity, Habit ) VALUES ( '{date}', {quantity}, 'coffee')";
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
				@$"SELECT * FROM habit_table";
			var reader = tableCmd.ExecuteReader();

			if (reader.HasRows)
			{
				while (reader.Read())
				{
					queryRecords.Add( new CoffeeRecord(reader.GetInt32(0),reader.GetString(1),reader.GetInt32(2),reader.GetString(3)));
				}
			} else { Console.WriteLine(CenterText("No data found\n")); }

			reader.Close();
			connection.Close();
		}
		if (queryRecords.Count > 0)
		{
			Console.WriteLine(CenterText("------------- Records -------------"));
			foreach (CoffeeRecord row in queryRecords)
			{
				Console.WriteLine(CenterText($"Id: {row.Id}\t  {row.Date}\tQuantity: {row.Quantity}"));
			}
			Console.WriteLine(CenterText("-----------------------------------\n"));
		}
		Console.WriteLine(CenterText("Press any key to exit"));
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

	static int GetNumInput(string message)
	{
		while (true)
		{
			Console.WriteLine(message);
			string? input = Console.ReadLine();
			if (input == "exit") { Menu(); }
			if (int.TryParse(input, out int result) && result > 0 && result < 100) { return result; }
			else { Console.WriteLine("Invalid quantity"); }
		}
	}

	static string GetDateInput()
	{
		while (true)
		{
			Console.WriteLine("Enter date (formatted mm-dd-yy)");
			string? input = Console.ReadLine();
			if (input == "exit") { Menu(); }
			if (DateTime.TryParseExact(input, "MM-dd-yy", new CultureInfo("en-US"), 0, out DateTime outyDate)) { return input; }
			else { Console.WriteLine("Invalid date"); }
		}
	}

	static void Animate(string[,] cells, int[] topLeftCoords, int threadSleep)
	{
		int[] bottomRightCoords = { topLeftCoords[0] + cells[0,0].Length - 1, topLeftCoords[1] + cells.GetLength(1) - 1 };
		for (int i = 0; i < cells.GetLength(0); i++)
		{
			ClearRange(topLeftCoords, bottomRightCoords);
			for (int j = 0; j < cells.GetLength(1); j++)
			{
				Console.SetCursorPosition(topLeftCoords[0], topLeftCoords[1] + j);
				Console.Write(cells[i, j]);
			}
			Thread.Sleep(threadSleep);
		}
	}

	static void ClearRange(int[] topLeftCoords, int[] bottomRightCoords)
	{
		int rowQuantity = bottomRightCoords[1] - topLeftCoords[1] + 1;
		int clearLength = bottomRightCoords[0] - topLeftCoords[0] + 1;
		for (int i = 0; i < rowQuantity; i++)
		{
			Console.SetCursorPosition(topLeftCoords[0], topLeftCoords[1] + i);
			Console.Write("".PadLeft(clearLength));
		}
	}

	static void SetWindow()
	{
		int targetW = 72;
		int targetH = 24;
		width = (targetW > Console.LargestWindowWidth) ? Console.LargestWindowWidth : targetW;
		height = (targetH > Console.LargestWindowHeight) ? Console.LargestWindowHeight : targetH;
		if (OperatingSystem.IsWindows())
		{
			Console.SetBufferSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
			Console.SetWindowSize(width, height);
			Console.SetBufferSize(width, height);
		}
		else
		{
			width = Console.WindowWidth;
			height = Console.WindowHeight;
		}
	}

	static string CenterText(string input)
	{
		return input.PadLeft(width / 2 + input.Length / 2);
	}

	static string CenterText(string input, int boundWidth)
	{
		return input.PadLeft(boundWidth / 2 + input.Length / 2);
	}

	static void HabitSelector(ConsoleKeyInfo readKey)
	{
		if (readKey.Key == ConsoleKey.RightArrow)
		{ currentHabitIndex = ((currentHabitIndex + habitTypes.Count + 1) % habitTypes.Count); }
		if (readKey.Key == ConsoleKey.LeftArrow)
		{ currentHabitIndex = ((currentHabitIndex + habitTypes.Count - 1) % habitTypes.Count); }
	}

	static void GetHabitTypes()
	{
		using (connection)
		{
			connection.Open();
			var tableCmd = connection.CreateCommand();
			tableCmd.CommandText =
				@$"SELECT * FROM habit_types";
			var reader = tableCmd.ExecuteReader();

			if (reader.HasRows)
			{
				while (reader.Read())
				{
					habitTypes.Add( reader.GetString(0) );
				}
			}
			reader.Close();
			connection.Close();
		}
	}

	static void CreateTablesIfNotExists()
	{
		string[] initialCmds =
		[
			@"CREATE TABLE IF NOT EXISTS habit_table (
				Id INTEGER PRIMARY KEY AUTOINCREMENT,
				Date TEXT,
				Quantity INTEGER,
				Habit TEXT
				)",
			@"CREATE TABLE IF NOT EXISTS habit_types (
				Type TEXT PRIMARY KEY
				)"
		];
		using (connection)
		{
			connection.Open();
			foreach (string cmd in initialCmds)
			{
				var tableCmd = connection.CreateCommand();
				tableCmd.CommandText = cmd;
				tableCmd.ExecuteNonQuery();
			}
			connection.Close();
		}
	}

	static void AddType()
	{
		//return current types from db
		//prompt for new
		//validate for string length
		//validate for not existing
		//add to db type table
		//add to type list
		//return to menu
	}
}

public class CoffeeRecord(int idIn, string dateIn, int quantityIn, string habit)
{
	public int Id { get; } = idIn;
	public string Date { get; } = dateIn;
	public int Quantity { get; } = quantityIn;
	public string Habit { get; } = habit;
}

