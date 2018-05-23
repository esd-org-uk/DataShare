# What is DataShare?
DataShare is a .NET application developed by the London Borough of Redbridge to enable an organisation to publish open data for downloading in tabular format or viewing online as tables, charts and maps.

See [Redbridge’s datashare](http://data.redbridge.gov.uk/) for an example of a live instance.

Redbridge made DataShare open source in March 2018. It may be used and modified subject to crediting the London Borough of Redbridge. Software support is provided as part of the Local Government Association’s (LGA) open data work via the LG Inform Plus programme.

Email the LGA's [LG Inform Plus Support team](mailto:support@esd.org.uk) if you need help.

# Installing DataShare
See the [DataShare Installation Guide](https://github.com/esd-org-uk/DataShare/blob/master/DataShare%20Installation%20Guide%20v1_4.pdf) for details. The installer uses [WiX](http://wixtoolset.org/) to install. It will expect a connection string to a SQL Server database, so an empty database needs to be set up in advance, along with the user who will access the database.

# Administering Datashare
The /admin page provides options to administer DataShare and lets a super administrator set up new users who can upload data, create/edit schemas (defining the structure of data) or act as super administrators themselves.  See the [DataShare Admin User Guide](https://github.com/esd-org-uk/DataShare/blob/master/DataShare%20Admin%20User%20Guide.pdf).

# Using DataShare
Datashare provides a simple way for councils to delegate publishing to staff in different departments who simply upload data in CSV format.

# Harvesting from your DataShare to data.gov.uk and the LGA
DataShare implements output of an inventory of all your published datasets compliant with [the inventory schema](http://schemas.opendata.esd.org.uk/Inventory). Inventories are generated in XML format from the /api/esdInventory address of each DataShare instance, eg http://data.redbridge.gov.uk/api/esdInventory .

Your datasets are automatically published at [Find open data](https://data.gov.uk/) (aka data.gov.uk) once you’ve configured it to harvest from your inventory.

![Find open data](https://e-sd.org/wDCMb/ "Find open data - Harvest source setup")

The same is true of the LGA’s catalogue of open datasets. Configure harvesting at http://inventories.opendata.esd.org.uk/harvesters/add 

![LG Inform Plus Open Data](https://e-sd.org/qkCCs/ "LG Inform Plus Open Data - Harvest source setup")

# Support
Post issues here or email support@esd.org.uk if you have queries on using DataShare or would like to see improvements.

The LGA’s technical partner, [Porism](http://porism.com/), can arrange to host an instance of DataShare for your council or other organisation.