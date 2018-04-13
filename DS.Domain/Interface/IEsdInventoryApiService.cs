using System.Collections.Generic;

namespace DS.Domain.Interface
{
    public interface IEsdInventoryApiService
    {
        //IRepository<Category> Repository { get; }

        /// <summary>
        /// root url of the datashare website
        /// </summary>
        string RootUrl { get; set; }

        /// <summary>
        /// Open Gov Licence url - i.e. http://www.nationalarchives.gov.uk/doc/open-government-licence
        /// </summary>
        string OpenGovLicenceUrl { get; set; }

        ///// <summary>
        ///// url with for the coverage of the dataset - spatial - i.e. http://statistics.data.gov.uk/id/statistical-geography/E09000026
        ///// </summary>
        //string SpatialUrl { get; set; }

        /// <summary>
        /// RightsHolder of the dataset
        /// </summary>
        string RightsHolder { get; set; }

        List<InventoryDataset> GetInventoryDataset();
    }
}