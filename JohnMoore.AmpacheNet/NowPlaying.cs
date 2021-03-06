//
// NowPlaying.cs
//
// Author:
//       John Moore <jcwmoore@gmail.com>
//
// Copyright (c) 2011 John Moore
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using JohnMoore.AmpacheNet.Logic;

namespace JohnMoore.AmpacheNet
{
	[Activity (Label = "@string/nowPlayingLabel")]
	[MetaData("android.app.default_searchable", Value = ".SongSearch")]
	public class NowPlaying : PlayingActivity, Android.Widget.SeekBar.IOnSeekBarChangeListener
	{
		private bool _listenForPlayingPositionUpdates = true;
		private double _requestedSeek;
        protected override int MenuId { get { return Resource.Menu.SubMenu; } }
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView(Resource.Layout.NowPlaying);
			var ori = WindowManager.DefaultDisplay.Rotation;
			if(WindowManager.DefaultDisplay.Height > WindowManager.DefaultDisplay.Width)
			{
				FindViewById<LinearLayout>(Resource.Id.mainLayout).Orientation = Orientation.Vertical;
			}
			_uiActions.Add(AmpacheModel.PLAYING_SONG, UpdateUi);
			_uiActions.Add(AmpacheModel.PERCENT_DOWNLOADED, UpdateDownloadProgress);
			_uiActions.Add(AmpacheModel.PERCENT_PLAYED, UpdatePlayerPosition);
			FindViewById<SeekBar>(Resource.Id.prgSeeking).SetOnSeekBarChangeListener(this);
		}

		void HandleSeekingTouch (object sender, View.TouchEventArgs e)
		{
			
		}
		
		protected override void OnModelLoaded ()
		{
			base.OnModelLoaded ();
			UpdateUi();
		}

		void UpdateUi()
		{
			Console.WriteLine ("Change UI");
			if(_model.PlayingSong == null)
			{
				FindViewById<TextView>(Resource.Id.lblNowPlayingTrack).Text = string.Empty;
				FindViewById<TextView>(Resource.Id.lblNowPlayingAlbum).Text = string.Empty;
				FindViewById<TextView>(Resource.Id.lblNowPlayingArtist).Text = string.Empty;
			}
			else
			{
				FindViewById<TextView>(Resource.Id.lblNowPlayingTrack).Text = _model.PlayingSong.Name;
				FindViewById<TextView>(Resource.Id.lblNowPlayingAlbum).Text = _model.PlayingSong.AlbumName;
				FindViewById<TextView>(Resource.Id.lblNowPlayingArtist).Text = _model.PlayingSong.ArtistName;
			}
			UpdateDownloadProgress();
			UpdatePlayerPosition();
		}
		
		void UpdateDownloadProgress()
		{
			FindViewById<ProgressBar>(Resource.Id.prgSeeking).SecondaryProgress = (int)_model.PercentDownloaded;
		}
		
		void UpdatePlayerPosition()
		{
			if(_listenForPlayingPositionUpdates)
			{
				FindViewById<ProgressBar>(Resource.Id.prgSeeking).Progress = (int)_model.PercentPlayed;
			}
		}

		#region IOnSeekBarChangeListener implementation
		public void OnProgressChanged (SeekBar seekBar, int progress, bool fromUser)
		{
			if(fromUser)
			{
				_requestedSeek = (double)progress / seekBar.Max;
			}
		}

		public void OnStartTrackingTouch (SeekBar seekBar)
		{
			_listenForPlayingPositionUpdates = false;
		}

		public void OnStopTrackingTouch (SeekBar seekBar)
		{
			_listenForPlayingPositionUpdates = true;
			Console.WriteLine (_requestedSeek.ToString());
			_model.RequestedSeekToPercentage = _requestedSeek;
		}
		#endregion


	}
}