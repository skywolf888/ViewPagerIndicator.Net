//package com.viewpagerindicator.sample;

//import android.os.Bundle;
//import android.support.v4.view.ViewPager;
//import com.viewpagerindicator.CirclePageIndicator;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.View;
using Com.ViewPagerIndicator;
using R = ViewPagerIndicator.Net.Sample.Resource;

namespace ViewPagerIndicator.Net.Sample
{
    [Activity(Label = "SampleCirclesSnap")]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new string[] { "com.jakewharton.android.viewpagerindicator.sample.SAMPLE" })]

    public class SampleCirclesSnap : BaseSampleActivity
    {
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.simple_circles);

            mAdapter = new TestFragmentAdapter(SupportFragmentManager);

            mPager = FindViewById<ViewPager>(R.Id.pager);
            mPager.Adapter=mAdapter;

            CirclePageIndicator indicator = (CirclePageIndicator)FindViewById(R.Id.indicator);
            mIndicator = indicator;
            indicator.setViewPager(mPager);
            indicator.setSnap(true);
        }
    }
}