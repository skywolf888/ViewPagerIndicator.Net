//package com.viewpagerindicator.sample;

//import android.os.Bundle;
//import android.support.v4.view.ViewPager;
//import com.viewpagerindicator.UnderlinePageIndicator;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Widget;
using Com.ViewPagerIndicator;
using R = ViewPagerIndicator.Net.Sample.Resource;

namespace ViewPagerIndicator.Net.Sample
{
    [Activity(Label = "SampleUnderlinesDefault")]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new string[] { "com.jakewharton.android.viewpagerindicator.sample.SAMPLE" })]

    public class SampleUnderlinesDefault : BaseSampleActivity
    {
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.simple_underlines);

            mAdapter = new TestFragmentAdapter(SupportFragmentManager);

            mPager = FindViewById<ViewPager>(R.Id.pager);
            mPager.Adapter = mAdapter;

            mIndicator = (UnderlinePageIndicator)FindViewById(R.Id.indicator);
            mIndicator.setViewPager(mPager);
        }
    }
}