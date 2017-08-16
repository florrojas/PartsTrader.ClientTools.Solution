using System;
using System.Collections.Generic;
using System.Linq;
using PartsTrader.ClientTools.Api;
using PartsTrader.ClientTools.Api.Data;
using PartsTrader.ClientTools.Integration;
using System.IO;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace PartsTrader.ClientTools
{
    public class PartCatalogue : IPartCatalogue
    {
        private readonly IPartsTraderPartsService _partsTraderPartsService; //Dependency

        public PartCatalogue(IPartsTraderPartsService partsTraderPartsService)
        {
            _partsTraderPartsService = partsTraderPartsService;
        }

        public IEnumerable<PartSummary> GetCompatibleParts(string partNumber)
        {
            //TODO: implement
            try
            {
                IEnumerable<PartSummary> _partSummaryList;

                //validate part number
                ValidatePartNumber(partNumber);
                //
                //check Exclusions list
                if (CheckExclusionList(partNumber))
                {
                    _partSummaryList = new List<PartSummary>();
                }else
                //call the partTraderService
                 _partSummaryList = _partsTraderPartsService.FindAllCompatibleParts(partNumber);

                //Return list of PartSummary
                return _partSummaryList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void ValidatePartNumber(string partNumber)
        {
            Regex reg = new Regex(@"^\d{4}-[a-zA-Z0-9]{4}[a-zA-Z0-9]{0,}");
            if(!reg.IsMatch(partNumber))
                throw new InvalidPartException("Invalid code!!");
        }

        private bool CheckExclusionList(string partNumber)
        {
            var _listExclusions = GetExclusionsFromJsonFile();
            if (_listExclusions.Any())
            {
                return (_listExclusions.Any(x => x.PartNumber.Equals(partNumber,StringComparison.InvariantCultureIgnoreCase))) ? true : false;
            }
            else
                return false;
        }

        private IEnumerable<PartSummary> GetExclusionsFromJsonFile()
        {
            string _exclusionsJson = File.ReadAllText("Exclusions.json");
            return JsonConvert.DeserializeObject<IEnumerable<PartSummary>>(_exclusionsJson);
        }
    }
    
}