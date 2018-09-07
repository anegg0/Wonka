﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace WonkaEth.Contracts
{
    /// <summary>
    /// 
    /// This class presents a collection of RuleTrees that can be executed 
    /// together and in sequence as one logical entity.
    /// 
    /// </summary>
    public class WonkaRuleGrove
    {
        public WonkaRuleGrove(string psGroveId)
        {
            GroveId = psGroveId;

            GroveDescription = OwnerId = "";

            OrderedRuleTrees = new List<WonkaRegistryItem>();
        }

        #region Properties

        public string GroveId { get; set; }

        public string GroveDescription { get; set; }

        public List<WonkaRegistryItem> OrderedRuleTrees { get; set; }

        public string OwnerId { get; set; }

        public uint TotalMinGasCost
        {
            get
            {
                uint nMinCost = 0;

                OrderedRuleTrees.ForEach(x => nMinCost += x.MinGasCost);

                return nMinCost;
            }
        }

        public uint MaxGasCost
        {
            get
            {
                uint nMaxCost = 0;

                OrderedRuleTrees.ForEach(x => nMaxCost += x.MaxGasCost);

                return nMaxCost;
            }
        }

        public HashSet<string> TotalRequiredAttributes
        {
            get
            {
                HashSet<string> TotalAttributes = new HashSet<string>();

                OrderedRuleTrees.ForEach(x => TotalAttributes.UnionWith(x.RequiredAttributes));

                return TotalAttributes;
            }
        }

        public uint CreationEpochTime;

        public DateTime CreationTime
        {
            get
            {
                DateTime ct = new DateTime(1970, 1, 1);

                ct = ct.AddSeconds(CreationEpochTime);

                return ct;
            }
        }

        #endregion
    }
}
