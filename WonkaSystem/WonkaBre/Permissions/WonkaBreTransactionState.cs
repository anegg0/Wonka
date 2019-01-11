﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WonkaBre.Permissions
{
    /// <summary>
    /// 
    /// This class is a provided implementation of the ITransactionstate interface.  It can contain all state data 
    /// in relation to the pending transaction of a RuleTree, which will be invoked within the rules engine.
    /// 
    /// </summary>
    public class WonkaBreTransactionState : ITransactionState
    {
        #region CONSTANTS

        public const int CONST_MAX_OWNERS = 250;

        #endregion

        public WonkaBreTransactionState(IEnumerable<string> poOwners, uint pnMinReqScoreForApproval = 0)
        {
            OwnerWeights = new Dictionary<string, uint>();

            foreach (string sTmpOwner in poOwners)
            {
                OwnerWeights[sTmpOwner] = 1;

                OwnerConfirmations[sTmpOwner] = false;
            }

            if (OwnerWeights.Keys.Count >= CONST_MAX_OWNERS)
                throw new WonkaBrePermissionsException("ERROR!  Too many owners were provided.  Maximum limit is [" + CONST_MAX_OWNERS + "].");

            if (OwnerWeights.Keys.Count == 0)
                throw new WonkaBrePermissionsException("ERROR!  No owners were provided.");

            MinReqScoreForApproval = (pnMinReqScoreForApproval > 0) ? pnMinReqScoreForApproval : (uint) OwnerWeights.Count / 2;
        }

        #region Methods

        public void AddConfirmation(string psOwner)
        {
            if (!IsOwner(psOwner))
                throw new WonkaBrePermissionsException("ERROR!  Account(" + psOwner + ") is not a registered owner that is associated with this RuleTree.");

            OwnerConfirmations[psOwner] = true;
        }

        public void ClearPendingTransaction()
        {
            RevokeAllConfirmations();

            // NOTE: In the case that we introduce othe state variables, they should be reset here
        }

        public uint GetCurrentScore()
        {
            uint nCurrentScore = 0;

            foreach (string sTmpOwner in OwnerConfirmations.Keys)
            {
                if (OwnerConfirmations[sTmpOwner])
                    nCurrentScore += OwnerWeights[sTmpOwner];
            }

            return nCurrentScore;
        }

        public uint GetMinScoreRequirement()
        {
            return MinReqScoreForApproval;
        }

        public HashSet<string> GetOwnersConfirmed()
        {
            HashSet<string> Confirmed = new HashSet<string>();

            OwnerConfirmations.Where(x => HasConfirmed(x.Key)).ToList().ForEach(x => Confirmed.Add(x.Key));

            return Confirmed;
        }

        public HashSet<string> GetOwnersUnconfirmed()
        {
            HashSet<string> Confirmed = new HashSet<string>();

            OwnerConfirmations.Where(x => !HasConfirmed(x.Key)).ToList().ForEach(x => Confirmed.Add(x.Key));

            return Confirmed;
        }

        public bool HasConfirmed(string psOwner)
        {
            if (!IsOwner(psOwner))
                throw new WonkaBrePermissionsException("ERROR!  Account(" + psOwner + ") is not a registered owner that is associated with this RuleTree.");

            return OwnerConfirmations[psOwner];
        }

        public bool IsOwner(string psOwner)
        {
            if (String.IsNullOrEmpty(psOwner))
                throw new WonkaBrePermissionsException("ERROR!  Provided owner cannot be null or blank.");

            return (OwnerWeights.ContainsKey(psOwner));
        }

        public bool IsTransactionConfirmed()
        {
            return (GetCurrentScore() >= MinReqScoreForApproval);
        }

        public void RemoveOwner(string psOwner)
        {
            if (!IsOwner(psOwner))
                throw new WonkaBrePermissionsException("ERROR!  Account(" + psOwner + ") is not a registered owner that is associated with this RuleTree.");

            OwnerWeights.Remove(psOwner);
            OwnerConfirmations.Remove(psOwner);
        }

        public bool RevokeAllConfirmations()
        {
            foreach (string sTmpOwner in OwnerConfirmations.Keys)
                RevokeConfirmation(sTmpOwner);

            return true;
        }

        public bool RevokeConfirmation(string psOwner)
        {
            if (!IsOwner(psOwner))
                throw new WonkaBrePermissionsException("ERROR!  Account(" + psOwner + ") is not a registered owner that is associated with this RuleTree.");

            OwnerConfirmations[psOwner] = false;

            return true;
        }

        public void SetMinScoreRequirement(uint pnMinReq)
        {
            if (pnMinReq == 0)
                throw new WonkaBrePermissionsException("Minimum requirement has to be greater than 0.");

            MinReqScoreForApproval = pnMinReq;
        }

        public void SetOwner(string psOwner, uint pnWeight = 1)
        {
            if (OwnerConfirmations.Count >= CONST_MAX_OWNERS)
                throw new WonkaBrePermissionsException("ERROR!  Max count of owners [" + CONST_MAX_OWNERS + "] has already been reached.");

            if (String.IsNullOrEmpty(psOwner))
                throw new WonkaBrePermissionsException("ERROR!  Provided owner cannot be null or blank.");

            OwnerConfirmations[psOwner] = false;
            OwnerWeights[psOwner]       = pnWeight;
        }

        #endregion

        #region Properties

        private uint MinReqScoreForApproval { get; set; }

        private Dictionary<string, bool> OwnerConfirmations { get; set; }

        private Dictionary<string, uint> OwnerWeights { get; set; }

        #endregion
    }
}
