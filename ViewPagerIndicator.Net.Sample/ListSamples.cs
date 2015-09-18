//package com.viewpagerindicator.sample;

//import java.text.Collator;
//import java.util.ArrayList;
//import java.util.Collections;
//import java.util.Comparator;
//import java.util.HashMap;
//import java.util.List;
//import java.util.Map;
//import android.app.ListActivity;
//import android.content.Intent;
//import android.content.pm.PackageManager;
//import android.content.pm.ResolveInfo;
//import android.os.Bundle;
//import android.view.View;
//import android.widget.ListView;
//import android.widget.SimpleAdapter;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace ViewPagerIndicator.Net.Sample
{
    [Activity(Label = "ViewPagerIndicator.Net.Sample", MainLauncher = true, Icon = "@drawable/icon")]
    public class ListSamples : ListActivity
    {

        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Android.Content.Intent intent = this.Intent;
            string path = intent.GetStringExtra("com.jakewharton.android.viewpagerindicator.sample.Path");

            if (path == null)
            {
                path = "";
            }

            ListAdapter = new SimpleAdapter(this, getData(path),
                    Android.Resource.Layout.SimpleListItem1, new string[] { "title" },
                    new int[] { Android.Resource.Id.Text1 });

            //getListView().setTextFilterEnabled(true);
            ListView.TextFilterEnabled = true;
            
        }

        protected IList<IDictionary<string, object>> getData(string prefix)
        {
            IList<IDictionary<string, object>> myData = new List<IDictionary<string, object>>();
             
            Android.Content.Intent mainIntent = new Intent(Android.Content.Intent.ActionMain, null);
            mainIntent.AddCategory("com.jakewharton.android.viewpagerindicator.sample.SAMPLE");

            Android.Content.PM.PackageManager pm = this.PackageManager;

            IList<ResolveInfo> list = pm.QueryIntentActivities(mainIntent, 0);

            if (null == list)
                return myData;

            string[] prefixPath;
            string prefixWithSlash = prefix;

            if (prefix.Equals(""))
            {
                prefixPath = null;
            }
            else
            {
                prefixPath = prefix.Split(new char[] { '/' });
                prefixWithSlash = prefix + "/";
            }

            int len = list.Count;

            

            IDictionary<string, bool> entries = new JavaDictionary<string, bool>();

            for (int i = 0; i < len; i++)
            {
                ResolveInfo info = list[i];
                string labelSeq = info.LoadLabel(pm);
                string label = labelSeq != null
                        ? labelSeq.ToString()
                        : info.ActivityInfo.Name;

                if (prefixWithSlash.Length == 0 || label.StartsWith(prefixWithSlash))
                {

                    string[] labelPath = label.Split(new char[] { '/' });

                    string nextLabel = prefixPath == null ? labelPath[0] : labelPath[prefixPath.Length];

                    if ((prefixPath != null ? prefixPath.Length : 0) == labelPath.Length - 1)
                    {
                        addItem(myData, nextLabel, activityIntent(
                                info.ActivityInfo.ApplicationInfo.PackageName,
                                info.ActivityInfo.Name));
                    }
                    else
                    {
                        if (entries[nextLabel] == false)
                        {
                            addItem(myData, nextLabel, browseIntent(prefix.Equals("") ? nextLabel : prefix + "/" + nextLabel));
                            entries.Add(nextLabel, true);
                        }
                    }
                }
            }
             

            //Collections.sort(myData, NAME_COMPARATOR);

            return myData;
        }       

        //private final static Comparator<Map<String, Object>> NAME_COMPARATOR =
        //    new Comparator<Map<String, Object>>() {
        //    private final Collator   collator = Collator.getInstance();

        //    public int compare(Map<String, Object> map1, Map<String, Object> map2) {
        //        return collator.compare(map1.get("title"), map2.get("title"));
        //    }
        //};

        protected Intent activityIntent(string pkg, string componentName)
        {
            Intent result = new Intent();
            result.SetClassName(pkg, componentName);
            return result;
        }

        protected Intent browseIntent(string path)
        {
            Intent result = new Intent();

            //result.SetClassName(this);
            //result.setClass(this, ListSamples.class);
            result.PutExtra("com.jakewharton.android.viewpagerindicator.sample.Path", path);
            return result;
        }

        protected void addItem(IList<IDictionary<string, object>> data, string name, Intent intent)
        {
            IDictionary<string, object> temp = new JavaDictionary<string, object>();
            temp.Add("title", name);
            temp.Add("intent", intent);
            data.Add(temp);
        }

        //@Override
        //@SuppressWarnings("unchecked")
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {

            IDictionary<string, object> map = (IDictionary<string, object>)l.GetItemAtPosition(position);
            Intent intent = (Intent)map["intent"];
            StartActivity(intent);
        }
    }
}