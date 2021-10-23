# masshunter-scripts
Utility scripts and report templates made for Agilent MassHunter suite. Scripts are generally stored in `MassHunter\Scripts\<Application>\`. Report templates are stored in `MassHunter\Report Templates\<Application>`. MassHunter SDK documentation has additional info.

### Unknowns
* **HitsOdorSearch.Unknowns.script** - Retrieves odor description from [Good Scents Company](http://www.thegoodscentscompany.com/) based on the name of identified substance.

### Unknowns Reporting
* **BatchPeaksCSV.cs** - Exports all hit peaks of Unknowns batch as a CSV file. Currently exports sample name, peak index, hit substance, area, height, RT, CAS and match score. Useful when exporting large amount samples for additional data analysis. Create your own report method with the template and export reports in batch mode.

### Library manager
* **PubchemCASFromName.libedit.script** - Searches current library entry name from Pubchem and replaces CAS and common name. 
