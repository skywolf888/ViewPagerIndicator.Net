//package com.viewpagerindicator.sample;

//import android.os.Bundle;
//import android.support.v4.view.ViewPager;
//import com.viewpagerindicator.LinePageIndicator;



using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View;
using Android.Widget;
using Com.ViewPagerIndicator;
using R = ViewPagerIndicator.Net.Sample.Resource;

namespace ViewPagerIndicator.Net.Sample
{
    [Activity(Label = "SampleLinesStyledTheme")]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new string[] { "com.jakewharton.android.viewpagerindicator.sample.SAMPLE" })]

    public class SampleLinesStyledTheme : BaseSampleActivity
    {
        //@Override
        protected void onCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //The look of this sample is set via a style in the manifest
            SetContentView(R.Layout.simple_lines);

            mAdapter = new TestFragmentAdapter(SupportFragmentManager);

            mPager = FindViewById<ViewPager>(R.Id.pager);
            mPager.Adapter = mAdapter;

            mIndicator = (LinePageIndicator)FindViewById(R.Id.indicator);
            mIndicator.setViewPager(mPager);
        }
    }
}