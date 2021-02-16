using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Nethereum.Util;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Nethereum.Web3.Accounts.Managed;
using stakingapi.Models;

namespace stakingapi.Controllers
{

    //[EnableCors(origins: "*", headers: "*", methods: "*")]

    public class apiController : ApiController
    {
        [HttpPost]
        public IHttpActionResult user_metamask(string metamask, double tokens_staked, double tokens_in_wallet)
        {
            string token = "";
            JWTAuthModel authModel = new JWTAuthModel();
            var context = new stakingapiEntities();
            var new_metamask = new stk_users();
            var metadetails = context.stk_users.Where(x => x.metamask == metamask).FirstOrDefault();

            using (DbContextTransaction dbTran = context.Database.BeginTransaction())
            {
                try
                {
                    if (metadetails == null)
                    {
                        new_metamask.metamask = metamask;
                        new_metamask.zin_in_wallet = tokens_in_wallet;
                        new_metamask.staked_zin = tokens_staked;
                        new_metamask.reward_percnetage = 0;
                        new_metamask.total_stk_percentage = 0;
                        new_metamask.total_reward = 0;
                        new_metamask.reward_per_frequency = 0;
                        new_metamask.timestamp = DateTime.UtcNow;
                        token = authModel.Authenticate(metamask);

                        context.stk_users.Add(new_metamask);
                        context.SaveChanges();
                        dbTran.Commit();
                    }
                    else {
                        metadetails.staked_zin = tokens_staked;
                        metadetails.zin_in_wallet = tokens_in_wallet;
                        context.SaveChanges();
                        dbTran.Commit();
                        token = authModel.Authenticate(metamask);
                    }
                }
                catch (Exception Ex)
                {
                    dbTran.Rollback();
                    return BadRequest(Ex.Message);
                }
            }
            return Json(new { status = 200, message = "success", result = token });

        }

        [HttpPost]
        public IHttpActionResult stake(string metamask, double transaction_value, double staked_tokens, double tokens_in_wallet, double total_staked_tokens)
        {
            var context = new stakingapiEntities();
            var metadetails = context.stk_users.Where(x => x.metamask == metamask).FirstOrDefault();
            var reward_per_frequency = context.stk_admin.FirstOrDefault();

            using (DbContextTransaction dbTran = context.Database.BeginTransaction())
            {
                try
                {
                    if (metadetails != null)
                    {
                        metadetails.staked_zin = staked_tokens;
                        metadetails.zin_in_wallet = tokens_in_wallet;
                        context.SaveChanges();

                        List<stk_users> results = (from metauser in context.stk_users
                                                   select metauser).ToList();

                        foreach (stk_users p in results)
                        {
                            p.total_stk_percentage = p.staked_zin == 0 ? 0 : (p.staked_zin / total_staked_tokens) * 100;
                            p.reward_per_frequency = p.staked_zin == 0 ? 0 : (p.staked_zin / total_staked_tokens) * reward_per_frequency.reward_per_frequency;
                            context.SaveChanges();
                            // editable from admin

                        }

                        dbTran.Commit();
                    }


                }
                catch (Exception Ex)
                {
                    dbTran.Rollback();
                    return BadRequest(Ex.Message);
                }
            }
            return Ok();

        }
        [HttpPost]
        public IHttpActionResult unstake(string metamask, double transaction_value, double staked_tokens, double tokens_in_wallet, double total_staked_tokens)
        {
            var context = new stakingapiEntities();
            var metadetails = context.stk_users.Where(x => x.metamask == metamask).FirstOrDefault();
            var reward_per_frequency = context.stk_admin.FirstOrDefault();

            using (DbContextTransaction dbTran = context.Database.BeginTransaction())
            {
                try
                {
                    if (metadetails != null)
                    {
                        metadetails.staked_zin = staked_tokens;
                        metadetails.zin_in_wallet = tokens_in_wallet;
                        context.SaveChanges();

                        List<stk_users> results = (from metauser in context.stk_users
                                                   select metauser).ToList();

                        foreach (stk_users p in results)
                        {
                            p.total_stk_percentage = p.staked_zin == 0 ? 0 : (p.staked_zin / total_staked_tokens) * 100;
                            p.reward_per_frequency = p.staked_zin == 0 ? 0 : (p.staked_zin / total_staked_tokens) * reward_per_frequency.reward_per_frequency; // editable from admin
                            context.SaveChanges();

                        }

                        dbTran.Commit();
                    }


                }
                catch (Exception Ex)
                {
                    dbTran.Rollback();
                    return BadRequest(Ex.Message);
                }
            }
            return Ok();

        }

        [HttpPost]
        public IHttpActionResult rewards(string metamask)
        {
            var context = new stakingapiEntities();
            var reward = context.stk_users.Where(x => x.metamask == metamask).FirstOrDefault();
            if (reward == null)
            {
                return BadRequest("Invalid Wallet address");
            }
            return Json(new { status = 200, message = "success", result = reward.total_reward });
        }
        [Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> withdraw(txencrypted txdetails)
        {
            if (StkHelper.checkTxHash(txdetails))
            {
                thashdetails thashresp = new thashdetails();
                var txHash = "";
                var context = new stakingapiEntities();
                var keys = context.stk_admin.FirstOrDefault();
                AdminKeys keystosend = new AdminKeys { };
                var metamaskdetails = context.stk_users.Where(x => x.metamask == txdetails.metamask).FirstOrDefault();
                if (metamaskdetails == null)
                {
                    return BadRequest("Invalid Wallet address");
                }
                var account = new Account(keys.private_key);
                var web3 = new Nethereum.Web3.Web3(account, "https://mainnet.infura.io/v3/14063be7fce843b18bfbd7beae73caca");
                try
                {
                    string apiUrl = "https://api.etherscan.io/api?module=account&action=txlist&address=0xfE276a83012Ff9C199109137BBCe864a225cefE9&startblock=" + txdetails.blockNumber.BN + "&endblock=" + txdetails.blockNumber.BN + "&sort=asc&apikey=98RSH7SEW3Z77D683QTJCU5M447IRD7S6I";
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(apiUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                        HttpResponseMessage response = await client.GetAsync(apiUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            var data = await response.Content.ReadAsStringAsync();
                            thashresp = Newtonsoft.Json.JsonConvert.DeserializeObject<thashdetails>(data);
                        }
                    }
                    if (thashresp != null)
                    {
                        //System.DateTime txTimestamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                        //txTimestamp = txTimestamp.AddSeconds(Convert.ToDouble(thashresp.result[0].timeStamp)).ToUniversalTime();
                        //if (thashresp.result[0].from == txdetails.metamask.ToLower() && thashresp.result[0].to == keys.stkcontractAddress && thashresp.result[0].value == txdetails.transactionFee.Transactionfee /*&& txTimestamp.AddMinutes(30) >= DateTime.UtcNow*/)
                        //{
                            var receiverAddress = txdetails.metamask;
                            var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();
                            var transfer = new TransferFunction()
                            {
                                To = receiverAddress,
                                TokenAmount = Web3.Convert.ToWei((BigDecimal)metamaskdetails.total_reward),
                                GasPrice = Nethereum.Web3.Web3.Convert.ToWei(txdetails.gasPrice.GasPrice, UnitConversion.EthUnit.Gwei),
                                Gas = txdetails.gasLimit.GasLimit
                            };

                            var transactionReceipt = await transferHandler.SendRequestAndWaitForReceiptAsync(keys.zinTokenAddress, transfer);
                            txHash = transactionReceipt.TransactionHash;
                            using (DbContextTransaction dbTran = context.Database.BeginTransaction())
                            {
                                try
                                {
                                    keystosend.rewards = metamaskdetails.total_reward;
                                    metamaskdetails.zin_in_wallet = metamaskdetails.zin_in_wallet + metamaskdetails.total_reward;
                                    metamaskdetails.total_reward = 0;
                                    context.SaveChanges();
                                    dbTran.Commit();

                                }
                                catch (Exception ex)
                                {
                                    dbTran.Rollback();
                                    return BadRequest(ex.Message);
                                }
                            }
                        //}
                    }
                    return Json(new { status = 200, message = "success", result = txHash });

                }
                catch (Exception Ex)
                {
                    var message = Ex.Message;
                    return BadRequest(Ex.InnerException.Message);
                }
                //return Json(new { status = 200, message = "success", result = "Ok" });
            }

            else
            {
                return BadRequest("Transaction details does not match with the blockchain provider");
            }


        }
        [Authorize]
        [HttpPost]
        public async Task<IHttpActionResult> estimatedGas(GasPayload gasdetails)
        {
            var context = new stakingapiEntities();
            var metamaskdetails = context.stk_users.Where(x => x.metamask == gasdetails.metamask).FirstOrDefault();
            if (metamaskdetails == null)
            {
                return BadRequest("Invalid Wallet address");
            }
            var keys = context.stk_admin.FirstOrDefault();
            var account = new Account(keys.private_key);
            var web3 = new Nethereum.Web3.Web3(account, "https://mainnet.infura.io/v3/14063be7fce843b18bfbd7beae73caca");
            var receiverAddress = gasdetails.metamask;
            var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();
            var transfer = new TransferFunction()
            {
                To = receiverAddress,
                TokenAmount = Web3.Convert.ToWei((BigDecimal)metamaskdetails.total_reward),
                GasPrice = Nethereum.Web3.Web3.Convert.ToWei(gasdetails.GasPrice, UnitConversion.EthUnit.Gwei),
                Gas = Convert.ToInt64(keys.gas_limit)
            };
            var estimate = await transferHandler.EstimateGasAsync(keys.zinTokenAddress, transfer);
            return Ok(estimate.Value);
        }
        [HttpPost]
        public IHttpActionResult getgaslimit(string contractAddress)
        {
            var context = new stakingapiEntities();
            var contractdetails = context.stk_admin.Where(x => x.public_address == contractAddress).FirstOrDefault();
            if (contractdetails == null)
            {
                return BadRequest("Invalid Wallet address");
            }

            return Ok(contractdetails.gas_limit);

        }

        [HttpPost]
        public IHttpActionResult reinvest(string metamask, double staked_tokens, double tokens_in_wallet, double total_staked_tokens/*(total staked tokens in system)*/)
        {
            var context = new stakingapiEntities();
            var metadetails = context.stk_users.Where(x => x.metamask == metamask).FirstOrDefault();
            var reward_per_frequency = context.stk_admin.FirstOrDefault();

            if (metadetails == null)
            {
                return BadRequest("Invalid Wallet Address");
            }

            using (DbContextTransaction dbTran = context.Database.BeginTransaction())
            {
                try
                {


                    metadetails.staked_zin = staked_tokens;
                    metadetails.zin_in_wallet = tokens_in_wallet;
                    metadetails.total_reward = 0;
                    context.SaveChanges();

                    List<stk_users> results = (from metauser in context.stk_users
                                               select metauser).ToList();

                    foreach (stk_users p in results)
                    {
                        p.total_stk_percentage = p.staked_zin == 0 ? 0 : (p.staked_zin / total_staked_tokens) * 100;
                        p.reward_per_frequency = p.staked_zin == 0 ? 0 : (p.staked_zin / total_staked_tokens) * reward_per_frequency.reward_per_frequency; // editable from admin
                        context.SaveChanges();

                    }

                    dbTran.Commit();

                }
                catch (Exception Ex)
                {
                    dbTran.Rollback();
                    return BadRequest(Ex.Message);
                }
            }
            return Ok();

        }

    }
}
