//package com.viewpagerindicator.sample;

//import java.util.Random;

//import com.viewpagerindicator.PageIndicator;

//import android.support.v4.app.FragmentActivity;
//import android.support.v4.view.ViewPager;
//import android.view.Menu;
//import android.view.MenuItem;
//import android.widget.Toast;


using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Com.ViewPagerIndicator;
using System;
using R = ViewPagerIndicator.Net.Sample.Resource;

namespace ViewPagerIndicator.Net.Sample
{
    public abstract class BaseSampleActivity : FragmentActivity
    {
        private static   Random RANDOM = new Random();

        protected TestFragmentAdapter mAdapter;
        protected ViewPager mPager;
        protected IPageIndicator mIndicator;

        //@Override
        public override bool OnCreateOptionsMenu(IMenu menu)
        {

            MenuInflater.Inflate(R.Menu.menu, menu);
            return true;

        }

        //@Override
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            
            switch (item.ItemId) {
                case R.Id.random:
                    int page = 0;
                    page = RANDOM.Next(mAdapter.Count);
                    Toast.MakeText(this, "Changing to page " + Convert.ToString(page), ToastLength.Short);
                    mPager.SetCurrentItem(page,true);
                    return true;

                case R.Id.add_page:
                    if (mAdapter.Count < 10) {
                        mAdapter.setCount(mAdapter.getCount() + 1);
                        mIndicator.notifyDataSetChanged();
                    }
                    return true;

                case R.Id.remove_page:
                    if (mAdapter.getCount() > 1) {
                        mAdapter.setCount(mAdapter.getCount() - 1);
                        mIndicator.notifyDataSetChanged();
                    }
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}