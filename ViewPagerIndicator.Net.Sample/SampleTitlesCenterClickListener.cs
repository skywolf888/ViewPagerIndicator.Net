//package com.viewpagerindicator.sample;

//import android.os.Bundle;
//import android.support.v4.view.ViewPager;
//import android.widget.Toast;

//import com.viewpagerindicator.TitlePageIndicator;
//import com.viewpagerindicator.TitlePageIndicator.IndicatorStyle;
//import com.viewpagerindicator.TitlePageIndicator.OnCenterItemClickListener;


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
    [Activity(Label = "SampleTitlesCenterClickListener")]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new string[] { "com.jakewharton.android.viewpagerindicator.sample.SAMPLE" })]

    public class SampleTitlesCenterClickListener : BaseSampleActivity, OnCenterItemClickListener
    {
        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(R.Layout.simple_titles);

            mAdapter = new TestFragmentAdapter(SupportFragmentManager);

            mPager = FindViewById<ViewPager>(R.Id.pager);
            mPager.Adapter = mAdapter;

            TitlePageIndicator indicator = (TitlePageIndicator)FindViewById(R.Id.indicator);
            indicator.setViewPager(mPager);
            indicator.setFooterIndicatorStyle(IndicatorStyle.Underline);
            indicator.setOnCenterItemClickListener(this);
            mIndicator = indicator;
        }

        //@Override
        public void onCenterItemClick(int position)
        {
            Toast.MakeText(this, "You clicked the center title!", ToastLength.Short).Show();
        }
    }
}