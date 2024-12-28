WanjaHaldemann_Projektauftrag-PROG1
Project Overview

This is a WPF-based project demonstrating key functionalities such as GUI design, database interaction, and file handling, developed using C# and .NET. The project adheres to the MVVM pattern, ensuring separation of concerns and maintainability.
Features

    Graphical User Interface:
        Modular UI components designed using XAML.
        Implements ergonomic feedback and input validation.
    Data Management:
        CRUD operations implemented using the repository and service patterns.
        Foreign key relationships and complex queries are managed with Entity Framework Core.
    File Handling:
        Import of data through CSV.
    Error Handling:
        Extensive use of try-catch blocks for robust error management.
    Planned Features:
        Multi-tenant database support.
        Encrypted communication with the database.

System Requirements

    The project must be run on the SmartLearn VM provided as part of the PROG1 module.
    A database server located on computer WIV11-VMWP must be available.
        Database Name: SwissAddresses
        Required Tables: Ensure the necessary tables for the project are created and available.

Installation Instructions

    Clone the repository:

    git clone https://github.com/HDMP/WanjaHaldemann_Projektauftrag-PROG1.git

    Open the project in Visual Studio.
    Restore NuGet packages.
    Build and run the application on the SmartLearn VM.

Database Configuration

    Ensure the database SwissAddresses is running on WIV11-VMWP.
    The application connects to the database using preconfigured connection strings in App.config (or similar configuration file).
    If necessary, update the connection string to match your environment:

    <connectionStrings>
        <add name="AppDbContext"
             connectionString="Server=WIV11-VMWP;Database=SwissAddresses;Trusted_Connection=True;" />
    </connectionStrings>

Branching Strategy

    Main Branch (main):
        Contains stable and tested code.
    Working Branch (working):
        Active development branch where features are added.
    Feature-Specific Branches:
        Individual features are developed on separate branches and merged into working upon completion.

Usage

    Start the Application:
        Launch the application through Visual Studio on the SmartLearn VM.
        Ensure the database on WIV11-VMWP is running and accessible.
        Navigate the UI for tasks like importing CSV data or managing records.
    Contribute:
        Clone the repository and create a new branch for your feature or fix.
        Submit a merge request after testing your changes.

Contributing

    Follow the branching strategy outlined above.
    Write clear and descriptive commit messages.
    Ensure all changes are tested before submitting a pull request.

Contact

For questions or suggestions, please contact the repository owner.
