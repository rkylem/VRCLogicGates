﻿
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

namespace ArchiTech
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [DefaultExecutionOrder(-1)]
    public class PlaylistSearch : UdonSharpBehaviour
    {
        public bool searchInTitles = true;
        public bool searchInUrls = false;
        public bool searchHiddenPlaylists = false;
        public bool searchOnEachKeypress = true;
        public Playlist[] playlistsToSearch;
        private InputField searchInput;

        private VRCPlayerApi local;
        private bool init = false;
        private bool hasSearchTargets = false;
        private bool debug = true;
        private string debugColor = "#ff9966";

        private void initialize()
        {
            if (init) return;
            hasSearchTargets = playlistsToSearch != null && playlistsToSearch.Length > 0;
            searchInput = GetComponentInChildren<InputField>();
            local = Networking.LocalPlayer;
            init = true;
        }

        void Start()
        {
            initialize();
        }

        public void _UpdateSearchOnKeypress()
        {
            if (searchOnEachKeypress) _UpdateSearch();
        }

        public void _UpdateSearch()
        {
            string searchTerm = searchInput.text.Trim();
            if (searchTerm != "")
                log($"Searching {playlistsToSearch.Length} playlists for '{searchTerm}'");
            searchTerm = searchTerm.ToLower();
            foreach (Playlist playlist in playlistsToSearch)
                if (playlist != null && (searchHiddenPlaylists || playlist.gameObject.activeInHierarchy))
                    filterPlaylist(playlist, searchTerm);
        }

        private int filterPlaylist(Playlist playlist, string searchTerm)
        {
            if (!playlist.init) return 0;
            var list = playlist.content;
            var titles = playlist.titles;
            var urls = playlist.urls;
            var hidden = new bool[urls.Length];
            if (searchTerm == "")
            {
                playlist._UpdateFilter(hidden);
                return urls.Length;
            }
            int count = 0;
            for (int i = 0; i < urls.Length; i++)
            {
                var shown = false;
                if (!shown && searchInTitles)
                {
                    var title = titles[i];
                    if (title != null) shown = titles[i].ToLower().Contains(searchTerm);
                }
                if (!shown && searchInUrls) {
                    var url = urls[i];
                    if (url != null) shown = url.Get().ToLower().Contains(searchTerm);
                }
                hidden[i] = !shown;
                if (shown) count++;
            }
            playlist._UpdateFilter(hidden);
            return count;
        }

        private void log(string value)
        {
            if (debug) Debug.Log($"[<color=#1F84A9>A</color><color=#A3A3A3>T</color><color=#2861B4>A</color> | <color={debugColor}>{nameof(PlaylistSearch)} ({name})</color>] {value}");
        }
        private void warn(string value)
        {
            if (debug) Debug.LogWarning($"[<color=#1F84A9>A</color><color=#A3A3A3>T</color><color=#2861B4>A</color> | <color={debugColor}>{nameof(PlaylistSearch)} ({name})</color>] {value}");
        }
        private void err(string value)
        {
            if (debug) Debug.LogError($"[<color=#1F84A9>A</color><color=#A3A3A3>T</color><color=#2861B4>A</color> | <color={debugColor}>{nameof(PlaylistSearch)} ({name})</color>] {value}");
        }
    }
}
