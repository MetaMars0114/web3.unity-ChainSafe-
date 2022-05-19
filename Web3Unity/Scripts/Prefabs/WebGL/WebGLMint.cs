using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;

#if UNITY_WEBGL
public class WebGLMint : MonoBehaviour
{
    // Start is called before the first frame update
    public string chain = "ethereum";
    public string network = "rinkeby"; // mainnet ropsten kovan rinkeby goerli
    public string account = "0x148dC439Ffe10DF915f1DA14AA780A47A577709E";
    public string to = "0x148dC439Ffe10DF915f1DA14AA780A47A577709E";
    public string cid = "QmXjWjjMU8r39UCEZ8483aNedwNRFRLvvV9kwq1GpCgthj";
    string chainId = "4";
    
    
    public async void MintNFT()
    {
        CreateMintModel.Response nftResponse = await EVM.CreateMint(chain, network, account, to, cid);

        // connects to user's browser wallet (metamask) to send a transaction
        try {
            string response = await WebGL.SendTransaction(nftResponse.tx.to, nftResponse.tx.value, nftResponse.tx.gasLimit, nftResponse.tx.gasPrice);
            print("Response: " + response);
        } catch (Exception e) {
            Debug.LogException(e, this);
        }
    }
}
#endif



