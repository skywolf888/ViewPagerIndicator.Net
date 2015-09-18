//package com.viewpagerindicator.sample;

//import android.os.Bundle;
//import android.support.v4.app.Fragment;
//import android.support.v4.app.FragmentActivity;
//import android.support.v4.app.FragmentManager;
//import android.support.v4.app.FragmentPagerAdapter;
//import android.support.v4.view.ViewPager;
//import com.viewpagerindicator.TabPageIndicator;


using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Widget;
using Com.ViewPagerIndicator;
using Java.Lang;
using R = ViewPagerIndicator.Net.Sample.Resource;

namespace ViewPagerIndicator.Net.Sample
{
    [Activity(Label = "SampleTabsDefault")]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new string[] { "com.jakewharton.android.viewpagerindicator.sample.SAMPLE" })]

    public class SampleTabsDefault : FragmentActivity
    {
        private static string[] CONTENT = new string[] { "Recent", "Artists", "Albums", "Songs", "Playlists", "Genres" };

        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.simple_tabs);

            FragmentPagerAdapter adapter = new GoogleMusicAdapter(SupportFragmentManager);

            ViewPager pager = FindViewById<ViewPager>(R.Id.pager);
            pager.Adapter = adapter;

            TabPageIndicator indicator = (TabPageIndicator)FindViewById(R.Id.indicator);
            indicator.setViewPager(pager);
        }

        class GoogleMusicAdapter : FragmentPagerAdapter
        {
            public GoogleMusicAdapter(Android.Support.V4.App.FragmentManager fm)
                : base(fm)
            {
                //super(fm);
            }

            //@Override
            public override Android.Support.V4.App.Fragment GetItem(int position)
            {
                return TestFragment.newInstance(CONTENT[position % CONTENT.Length]);
            }

            

            public override ICharSequence GetPageTitleFormatted(int position)
            {
                return new String(CONTENT[position % CONTENT.Length].ToUpper());
            }

            public override int Count
            {
                get { return CONTENT.Length; }
            }
            //@Override
            public int getCount()
            {
                return CONTENT.Length;
            }
        }
    }
}