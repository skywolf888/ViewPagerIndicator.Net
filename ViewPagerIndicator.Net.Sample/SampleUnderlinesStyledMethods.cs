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
    [Activity(Label = "SampleUnderlinesStyledMethods.Net.Sample")]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new string[] { "com.jakewharton.android.viewpagerindicator.sample.SAMPLE" })]

    public class SampleUnderlinesStyledMethods : BaseSampleActivity
    {
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.simple_underlines);

            mAdapter = new TestFragmentAdapter(SupportFragmentManager);

            mPager = FindViewById<ViewPager>(R.Id.pager);
            mPager.Adapter = mAdapter;

            UnderlinePageIndicator indicator = (UnderlinePageIndicator)FindViewById(R.Id.indicator);
            mIndicator = indicator;
            indicator.setViewPager(mPager);
            indicator.setSelectedColor(new Color(0xCC0000));
            indicator.SetBackgroundColor(new Color(0xCCCCCC));
            indicator.setFadeDelay(1000);
            indicator.setFadeLength(1000);
        }
    }
}