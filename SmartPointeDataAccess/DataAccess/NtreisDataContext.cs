using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartPointe.DataAccess
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;
    using SmartPointe.DTOs;
    using SmartPointe.Settings;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Xml.Linq;
    using System.Xml.XPath;

    public class NtreisDataContext : DbContext
    {
        IOptionsMonitor<SPOptions> _optionsAccessor;

        public NtreisDataContext(DbContextOptions<NtreisDataContext> options)
            : base(options)
        { }
    }

    public class NtreisRETS : IDisposable
    {
        HttpClient _client = null;

        //configuration.GetValue<bool>("FeatureToggles:DeveloperExceptions")

        public NtreisRETS(string username, string password)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.Credentials = new NetworkCredential(username,
                password);
            _client = new HttpClient(handler);
            
        }

        public async Task<int> TotalProps(NtreisRETsOptions options)
        {
                string results;

                StringBuilder urlBuilder = new StringBuilder(options.BaseUrl);
                urlBuilder.Append("search.ashx?");
                urlBuilder.Append(options.ClassString);
                urlBuilder.Append(options.Class);
                urlBuilder.Append(options.SearchTypeString);
                urlBuilder.Append(options.SearchType);
                urlBuilder.Append(options.QueryTypeString);
                urlBuilder.Append(options.QueryType);
                urlBuilder.Append(options.QueryString);
                urlBuilder.Append(options.Query);
                if (options.QueryActive)
                    urlBuilder.Append(",(status=A,P)");
                else
                    urlBuilder.Append(",(status=S)");

                urlBuilder.Append(options.StandardNames);
                urlBuilder.Append(options.LimitString);
                urlBuilder.Append(options.Limit);
                urlBuilder.Append(options.IncludeTotals);

                HttpResponseMessage response = _client.GetAsync(urlBuilder.ToString()).Result;
                HttpContent content = response.Content;

                results = await content.ReadAsStringAsync();

                XPathDocument xml = new XPathDocument(new StringReader(results));
                XPathNavigator xmlNav = xml.CreateNavigator();
                List<ResidentialProperty> rProps = new List<ResidentialProperty>();


                XPathNodeIterator xmlNavNode = xmlNav.Select("//COUNT");
                int recordsFound = 0;

                if (xmlNavNode.MoveNext())
                {
                    recordsFound = int.Parse(xmlNavNode.Current.GetAttribute("Records",""));
                }

                return recordsFound;
            }
        public async Task<PagedResult<ResidentialProperty>> Search(NtreisRETsOptions options)
        {
            string results;
            StringBuilder columnBuilder = new StringBuilder();
            StringBuilder dataBuilder = new StringBuilder();

            String[] columns = null;
            String[] data = null;

            long recordsFound = 0;
            int tabSpaces = 0;

            StringBuilder urlBuilder = new StringBuilder(options.BaseUrl);
            urlBuilder.Append("search.ashx?");
            urlBuilder.Append(options.ClassString);
            urlBuilder.Append(options.Class);
            urlBuilder.Append(options.SearchTypeString);
            urlBuilder.Append(options.SearchType);
            urlBuilder.Append(options.QueryTypeString);
            urlBuilder.Append(options.QueryType);
            urlBuilder.Append(options.QueryString);
            urlBuilder.Append(options.Query);
            if (options.QueryActive)
                urlBuilder.Append(",(status=A,P,AC,AOC,AKO,T,WS,W)");
            else
                urlBuilder.Append(",(status=S)");

            urlBuilder.Append(options.StandardNames);
            urlBuilder.Append(options.LimitString);
            urlBuilder.Append(options.Limit);

            urlBuilder.Append(options.OffsetString);
            urlBuilder.Append(options.Offset);

            HttpResponseMessage response = _client.GetAsync(urlBuilder.ToString()).Result;
            HttpContent content = response.Content;

            results = await content.ReadAsStringAsync();

            XPathDocument xml = new XPathDocument(new StringReader(results));
            XPathNavigator xmlNav = xml.CreateNavigator();
            
            List<ResidentialProperty> rProps = new List<ResidentialProperty>();

            
            XPathNodeIterator xmlNavNode = xmlNav.Select("//RETS/COUNT");
            if (xmlNavNode.MoveNext())
            {
                recordsFound = int.Parse(xmlNavNode.Current.GetAttribute("Records", ""));
            }

            xmlNavNode = xmlNav.Select("//RETS/DELIMITER");
            if (xmlNavNode.MoveNext())
            {
                tabSpaces = int.Parse(xmlNavNode.Current.GetAttribute("value", ""));
            }

            xmlNavNode = xmlNav.Select("//RETS/COLUMNS");
            if (xmlNavNode.MoveNext())
            {
                //columnBuilder.Append(xmlNavNode.Current.Value);
                columns = xmlNavNode.Current.Value.Split('\t');
            }

            xmlNavNode = xmlNav.Select("//RETS/DATA");
            int x = 0;
            while (xmlNavNode.MoveNext())
            {
                dataBuilder.Clear();
                //dataBuilder.Append(xmlNavNode.Current.Value);
                data = xmlNavNode.Current.Value.Split('\t');
                //data = dataBuilder.ToString().Split(new string(' ', tabSpaces));

                if (data.Length > 0)
                {
                    rProps.Add(new ResidentialProperty());

                    for (int z = 0; z < data.Length; z++)
                    {

                        switch (columns[z])
                        {
                            case "NumberOfStories":
                                rProps[x].STORIES = data[z];
                                break;
                            case "PropertyType":
                                rProps[x].PROPERTY_TYPE = data[z];
                                break;
                            case "MLSNumber":
                                rProps[x].MLS_ID = data[z];
                                break;
                            case "Matrix_Unique_ID":
                                rProps[x].MATRIX_ID = data[z];
                                break;
                            case "StreetDirPrefix":
                                rProps[x].STREET_DIR_PREFIX = data[z];
                                break;
                            case "StreetDirSuffix":
                                rProps[x].STREET_SUFFIX = data[z];
                                break;
                            case "StreetName":
                                rProps[x].STREET_NAME = data[z];
                                break;
                            case "StreetNumber":
                                rProps[x].STREET_NUMBER = data[z];
                                break;
                            case "City":
                                rProps[x].CITY = data[z];
                                break;
                            case "StateOrProvince":
                                rProps[x].STATE = data[z];
                                break;
                            case "PostalCode":
                                rProps[x].ZIPCODE = data[z];
                                break;
                            case "PoolFeatures":
                                rProps[x].POOL_FEATURES = data[z];
                                break;
                            case "PublicRemarks":
                                rProps[x].PUBLIC_REMARKS = data[z];
                                break;
                            case "Status":
                                rProps[x].STATUS = data[z];
                                break;
                            case "ListPrice":
                                rProps[x].ZIPCODE = data[z];
                                break;
                            case "ListOfficeName":
                                rProps[x].LISTING_OFFICE = data[z];
                                break;
                            case "ListOfficeMLSID":
                                rProps[x].LISTING_OFFICE_MLS = data[z];
                                break;
                            case "ParkingSpacesGarage":
                                rProps[x].GARAGE_PARK_SPACES = data[z];
                                break;
                            case "LotSizeAreaSQFT":
                                rProps[x].LOTSIZE = data[z];
                                break;
                            case "SqFtTotal":
                                rProps[x].SQFT = data[z];
                                break;
                            case "BedsTotal":
                                rProps[x].BEDS_TOTAL = data[z];
                                break;
                            case "BathsFull":
                                rProps[x].BATHS_FULL = data[z];
                                break;
                            case "BathsHalf":
                                rProps[x].BATHS_HALF = data[z];
                                break;
                            case "BathsTotal":
                                rProps[x].BATHS_TOTAL = data[z];
                                break;
                            case "PhotoCount":
                                rProps[x].PHOTO_COUNT = data[z];
                                break;
                            case "ListOfficePhone":
                                rProps[x].LISTING_OFFICE_PHONE = data[z];
                                break;
                            case "SubdivisionName":
                                rProps[x].SUBDIVISION = data[z];
                                break;
                            case "CountyOrParish":
                                rProps[x].COUNTY = data[z];
                                break;
                            case "InternetExposure":
                                rProps[x].HIDE_PROPERTY = data[z];
                                break;
                            case "Latitude":
                                rProps[x].LATITUDE = data[z];
                                break;
                            case "Longitude":
                                rProps[x].LONGITUDE = data[z];
                                break;

                        }
                    }

                    x++;
                }
            }

            return new PagedResult<ResidentialProperty>(rProps, options.Offset, options.Limit, recordsFound);

        }


        public async Task<byte[]> GetImage(NtreisRETsOptions options)
        {
            StringBuilder urlBuilder = new StringBuilder(options.BaseUrl);
            
            HttpResponseMessage response = _client.GetAsync(urlBuilder.ToString()).Result;
            HttpContent content = response.Content;

            var results = await content.ReadAsByteArrayAsync();

            return results;
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }

        }
    }

    public class NtreisRETsOptions
    {
        public NtreisRETsOptions()
        {
            // Set default value.
            ClassString = "CLASS=";
            Class = "Listing";
            SearchTypeString = "&searchType=";
            SearchType = "Property";
            QueryTypeString = "&queryType=";
            QueryType = "DMQL2";
            QueryString = "&Query=";
            Query = "(matrix_unique_ID=0%2B)";
            StandardNames = "&StandardNames=0&count=1&Format=COMPACT-DECODED";
            LimitString = "&Limit=";
            OffsetString = "&offset=";
            Offset = 1;
            Limit = 10;
            IncludeTotals = "&count=2";
            
        }

        public string Class { get; set; }
        public string SearchType { get; set; }
        public string QueryType { get; set; }
        public string Query { get; set; }
        public int QueryMlsId { get; set; }
        public bool QueryActive { get; set; }
        public int Limit { get; set; }
        public string LimitString { get; }
        public string OffsetString
        {
            get; set;
        }
        public int Offset { get; set; }
        public string ClassString { get; }
        public string SearchTypeString { get; }
        public string QueryTypeString { get; }
        public string QueryString { get; }
        public string StandardNames { get; }
        public string IncludeTotals { get; set; }




        public string BaseUrl { get; set; }
    }
}
