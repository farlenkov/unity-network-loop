using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

namespace UnityNetworkLoop
{
    public class SendDisconnectOnQuit : MonoBehaviour
    {
        Coroutine DisconnectCoroutine;
        bool IsReadyToQuit;

        void Start()
        {
            Application.wantsToQuit += WantsToQuit;
        }

        bool WantsToQuit()
        {
            Log.InfoEditor(
                "[SendDisconnectOnQuit: WantsToQuit] DisconnectCoroutine: {0}",
                DisconnectCoroutine != null);

            if (IsReadyToQuit)
                return true;

            if (DisconnectCoroutine == null)
                DisconnectCoroutine = StartCoroutine(Disconnect());

            return false;
        }

        IEnumerator Disconnect()
        {
            NetworkDriverManager.Current.Disconnect();
            yield return new WaitForSeconds(0.1f);

            IsReadyToQuit = true;
            Application.Quit();
        }
    }
}