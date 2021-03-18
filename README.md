# Iso20022ToEburyFiles
Conversion of ISO20022 PAIN.001.001.03 and PAIN.001.003.03 files to Ebury standard Mass Payment format

Verson 1.1.*
The version is tested for Ebury files created for the conversion formats above and major European currencies only
It will not create valid Ebury CSV files for niche currencies requiring special data, for example RUB and CNY

In all circumstances, the user remains responsible for the format and content of the output file. 

Only very limited validation is implemented in this application so if source data in ISO XML is incomplete or incorrect, then the resulting output CSV will also be invalid

How to use the application:

Geting Started:
On initial installation, go to the Setup menu, bottom left corner of the UI
	Set Input and Output Folders
		Input folder is the default location for the XML files and is best set to the top of your source data file structure
		Output folder gives you the option to have an equivalent tree containing output CSV (if you set a different top node) or to have the output CSV created adjacent to the input XML

Using the Home Screen:
	Convert File button will allow you to select a single XML file which is automatically converted
	The output file is shown after conversion, along with the contents in the lower secton of the screen for easy sense checking
	Double click the Output File name to review the file (if you have Excel or similar installed)

	You can Drag and Drop one or more XML files onto the application Home Screen to convert several files
		The summary of the last file only is shown after conversion
		The Drop works only away from the input boxes for Input and Output file

	The output is always named identically to the input with a .csv extension
		Previouly existing files are over-written
	The output location is in the Output Path plus the same sub-path as the Input file, for example
		Setting:
		Input Path:  G:\paymentFiles\XMLRoot
		Output Path: G:\paymentFiles\CSVRoot
		
		File Processing:
		Input File:  G:\paymentFiles\XMLRoot\customer1\Oct\File.xml
		Output file: G:\paymentFiles\CSVRoot\customer1\Oct\File.csv

		If the input file is not a sub-path of the Input Path, then the Output will be in the same folder as Input, just with the .csv extension.

Technical Information:
	This is a Prism based MVVM WPF application
	The included test project uses XUNIT
	Output depends on CsvHelper 
	Newtonsoft JSON is used throughout

No user or payment information is stored in this application after it is closed.
Any information in memory from a previous run is cleared from memory when a new file is converted or the application is closed.
The application persists the folders you configure and no other information.