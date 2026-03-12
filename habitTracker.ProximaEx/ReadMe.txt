	READ ME			
------------------------

How It Works
	magic

Thought Process
	concept: I'm drinking a lot of coffee lately,
		bet it could be more if i tried
	had to follow cap's tutorial to get bearings
	> one piece of it that really stood out was the SELECT EXISTS operation
		'SELECT EXISTS (SELECT 1 FROM drinking_wa...' I couldn't find specific
		documentation for it, but trying to understand what it returned and the
		syntax making it work was interesting.
	used primary constructor assignments in CoffeeRecord class for simplicity and omitted sets.
	tried to oversimplify get input into one method, for validation I decided this was the wrong move
	MVP is done, todo list look like this:

		----- Project Reqs -----
		> You should handle all possible errors so that the application never crashes.
		> Follow the DRY Principle, and avoid code repetition.
		> Your project needs to contain a Read Me file where
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
		> Write unit tests?
		> Parameterized queries?
		> Allow for multiple habits in one table, let users create habits and specify the units they are tracked in.
		> Seed data when created for the first time. Several habits and inserting 100 records automatically

What Was Hard
	viewing SQL db files
	setting working directory
	understanding how SQL syntax translates to literal function
	understanding C# to SQL translation
	wrapping my head around SQL data compiled format

What Was Easy
	C# syntax and code structure
	using methods
	logical operation
	thinking in terms of tables (col & row)

What Have I Learned
	using sqlite types and methods to interact with sql dbs
	basic sql commands like INSERT, SELECT, and DELETE
	basic sql formatting and order of ops
	creating and referencing a persistent local database
	that you can escape to start of a method by calling that method from within itself
