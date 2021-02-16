using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace stakingapi.Models
{
    public class PartialClasses
    {
        public class MetamaskUser
        {
            public string metamask { get; set; }
            public double staked_zin { get; set; }
            public double zin_in_wallet { get; set; }
        }
    }
    public class thashdetails
    {
        public string status { get; set; }
        public string message { get; set; }
        public Result[] result { get; set; }
    }

    public class Result
    {
        public string blockNumber { get; set; }
        public string timeStamp { get; set; }
        public string hash { get; set; }
        public string nonce { get; set; }
        public string blockHash { get; set; }
        public string transactionIndex { get; set; }
        public string from { get; set; }
        public string to { get; set; }
        public Int64 value { get; set; }
        public string gas { get; set; }
        public Int64 gasPrice { get; set; }
        public string isError { get; set; }
        public string txreceipt_status { get; set; }
        public string input { get; set; }
        public string contractAddress { get; set; }
        public string cumulativeGasUsed { get; set; }
        public string gasUsed { get; set; }
        public string confirmations { get; set; }
    }

    public class GasPayload
    {
        public string metamask { get; set; }
        public Int32 GasPrice { get; set; }

    }
    public class txencrypted
    {
        public string metamask { get; set; }
        public Transactionhash transactionHash { get; set; }
        public Blocknumber blockNumber { get; set; }
        public Gasprice gasPrice { get; set; }
        public Gaslimit gasLimit { get; set; }
        public TransactionFee transactionFee { get; set; }
    }

    public class Transactionhash
    {
        public string key { get; set; }
        public string txhash { get; set; }
    }

    public class Blocknumber
    {
        public string key { get; set; }
        public int BN { get; set; }
    }

    public class Gasprice
    {
        public string key { get; set; }
        public Int32 GasPrice { get; set; }
    }

    public class Gaslimit
    {
        public string key { get; set; }
        public Int64 GasLimit { get; set; }
    }

    public class TransactionFee
    {
        public string key { get; set; }
        public Int64 Transactionfee { get; set; }
    }


}