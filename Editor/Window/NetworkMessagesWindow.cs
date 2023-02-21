using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityGameLoop;

namespace UnityNetworkLoop
{
    public class NetworkMessagesWindow : EditorWindow
    {
        Dictionary<ushort, int> ReliableCounters;
        Dictionary<ushort, int> UnreliableCounters;
        Dictionary<ushort, int> TempCounters;
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
            ref Dictionary<ushort, int> max_counters)
        {
            if (max_counters == null)
                max_counters = new Dictionary<ushort, int>();

            if (TempCounters == null)
                TempCounters = new Dictionary<ushort, int>();
            else
                TempCounters.Clear();

            for (var i = 0; i < messages.Count; i++)
            {
                var message = messages[i];

                if (!TempCounters.TryGetValue(message.EventID, out var count))
                    TempCounters.Add(message.EventID, 1);
                else
                    TempCounters[message.EventID] = count + 1;
            }

            foreach (var current_counter in TempCounters)
            {
                if (!max_counters.TryGetValue(current_counter.Key, out var max_count))
                    max_counters.Add(current_counter.Key, current_counter.Value);
                else
                    if (max_count < current_counter.Value)
                        max_counters[current_counter.Key] = current_counter.Value;
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

            GUILayout.EndHorizontal();
        }

        void Draw(
            string title,
            Dictionary<ushort, int> counters)
        {
            if (counters == null)
                return;

            GUILayout.BeginVertical(GUILayout.Width(90));
            GUILayout.Label($"{title}");

            foreach (var counter in counters)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label($"{counter.Key}", GUILayout.Width(40));
                GUILayout.Label($"{counter.Value}", GUILayout.Width(40));
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }
    }
}