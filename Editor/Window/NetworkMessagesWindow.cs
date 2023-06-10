using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityGameLoop;
using UnityUtility;

namespace UnityNetworkLoop
{
    public class NetworkMessagesWindow : EditorWindow
    {
        Dictionary<ushort, Counter> ReliableCounters;
        Dictionary<ushort, Counter> UnreliableCounters;
        Dictionary<ushort, int> TempCounters;
        Dictionary<ushort, int> TempSizes;

        public class Counter
        {
            public int Count;
            public int Size;
        }

        bool IsUpdated;

        [MenuItem("Window/Network Messages")]
        static void Open()
        {
            GetWindow<NetworkMessagesWindow>("Network Messages");
        }

        void Update()
        {
            if (IsUpdated)
            {
                IsUpdated = false;
                Repaint();
            }
        }

        void SetNetworkLoop(NetworkLoop loop)
        {
            IsUpdated = true;

            UpdateCounters(
                loop.ReliableMessages,
                ref ReliableCounters);

            UpdateCounters(
                loop.UnreliableMessages,
                ref UnreliableCounters);
        }

        void UpdateCounters(
            NetworkMessageList messages,
            ref Dictionary<ushort, Counter> maxCounters)
        {
            if (maxCounters == null)
                maxCounters = new Dictionary<ushort, Counter>();

            DictionaryExt.Reset(ref TempCounters);
            DictionaryExt.Reset(ref TempSizes);

            for (var i = 0; i < messages.Count; i++)
            {
                var message = messages[i];

                if (!TempCounters.TryGetValue(message.EventID, out var count))
                    TempCounters.Add(message.EventID, 1);
                else
                    TempCounters[message.EventID] = count + 1;

                if (!TempSizes.TryGetValue(message.EventID, out var size))
                    TempSizes.Add(message.EventID, message.Length);
                else
                    if (size < message.Length)
                    TempSizes[message.EventID] = message.Length;
            }

            foreach (var currentCounter in TempCounters)
            {
                var messageId = currentCounter.Key;
                var messageCount = currentCounter.Value;

                if (!maxCounters.TryGetValue(messageId, out var max_counter))
                    maxCounters.Add(messageId, new Counter() { Count = messageCount });
                else
                    if (max_counter.Count < messageCount)
                    max_counter.Count = messageCount;
            }

            foreach (var currentCounter in TempSizes)
            {
                var messageId = currentCounter.Key;
                var messageSize = currentCounter.Value;

                if (!maxCounters.TryGetValue(messageId, out var max_counter))
                    maxCounters.Add(messageId, new Counter() { Size = messageSize });
                else
                    if (max_counter.Size < messageSize)
                    max_counter.Size = messageSize;
            }
        }

        void OnGUI()
        {
            if (NetworkMessagesInspectSystem.SetNetworkLoop != SetNetworkLoop)
                NetworkMessagesInspectSystem.SetNetworkLoop = SetNetworkLoop;

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            Draw("Reliable", ReliableCounters);
            Draw("Unreliable", UnreliableCounters);

            if (GUILayout.Button("Reset"))
            {
                ReliableCounters.Clear();
                UnreliableCounters.Clear();
            }

            GUILayout.EndHorizontal();
        }

        void Draw(
            string title,
            Dictionary<ushort, Counter> counters)
        {
            if (counters == null)
                return;

            GUILayout.BeginVertical(GUILayout.Width(90));
            GUILayout.Label($"{title}");

            foreach (var counter in counters)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{counter.Key}", GUILayout.Width(26));
                GUILayout.Label($"{counter.Value.Count} ({counter.Value.Size})", GUILayout.Width(60));
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
    }
}