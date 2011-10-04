# this assume that the nuget command line is on the path
# instead of running this file you can 
#	- open the solution
#	- right-click on the solution in the solution explorer
#	- select Enable Package Restore

Get-Item **\packages.config | ForEach-Object { & nuget install $_.FullName -o packages }