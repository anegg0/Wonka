﻿using System;
using System.Collections;

using WonkaBre.RuleTree;

namespace WonkaEth.Contracts
{
    public interface ISerialize
    {
        void DeserializeRecordFromBlockchain(ICommand poCommand);

        Nethereum.Contracts.Contract GetContract(WonkaBreSource poBlockchainSource);

        void SerializeRecordToBlockchain(ICommand poCommand);
    }
}