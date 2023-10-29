using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace System.Windows.Forms.DataVisualization.Charting
{
    internal static class DownSampling
    {
        internal static PointD[] DownsampleLTTB(PointD[] array, int length, bool dynamicX = true)
        {
            //Sanity Check
            if (array == null) return null;
            if (array.Length < length) return array;
            if (length < 2) return array;

            //Largest-Triangle-Three Bucket Downsampling 
            //Technical Reference: https://github.com/sveinn-steinarsson/highcharts-downsample
            //Code Reference: https://gist.github.com/adrianseeley/264417d295ccd006e7fd

            //Implementation: https://www.codearteng.com/2020/08/implementation-of-downsampling.html

            //With dynamicX, Actual data size returned might be less, Handle data point with no data by not returning 0,0. Use Dynamic List
            PointD[] window = new PointD[length];
            int w = 0;
            int data_size_less_start_and_end = length - 2;

            double bucket_size = (double)(array.Length - 2) / data_size_less_start_and_end;
            int a = 0;
            int next_a = 0;
            PointD max_area_point = new PointD();
            window[w++] = array[a]; // Always add the first point

            int start = 0, end = 0;
            int bucket_start = 1, bucket_end = 1;

            //Dynamic X Parameters (For data points where X incremental is not constant)
            double xStart = (array[1].X + array[0].X) / 2; //Mid X point for first 2 points
            double xEnd = (array[array.Length - 1].X + array[array.Length - 2].X) / 2; //Mid X point for last 2 points
            double bucketSizeX = (xEnd - xStart) / data_size_less_start_and_end; //Calculate bucket X width exclude first and last points
            double bucketBoundary = xStart + bucketSizeX;   //Next bucket boundary

            if (dynamicX)
            {
                while (array[bucket_end].X < bucketBoundary)
                {
                    bucket_end++;
                }
            }
            else
            {
                bucket_end = (int)(Math.Floor(bucket_size) + 1);   //End index of current bucket
            }
            //Debug.WriteLine("Bucket Start: " + bucket_start + ", " + bucket_end);

            for (int i = 0; i < data_size_less_start_and_end; i++)
            {
                // Calculate point average for next bucket (containing c)
                double avg_x = 0;
                double avg_y = 0;
                if (dynamicX)
                {
                    while (array[start].X < bucketBoundary)
                    {
                        start++;
                        if (start == array.Length) break;
                    }
                    bucketBoundary += bucketSizeX;
                    end = start;
                    if (end <= (array.Length - 1)) //Prevent buffer overrun
                    {
                        while (array[end].X < bucketBoundary)
                        {
                            end++;
                            if (end == array.Length) break;
                        }
                    }
                }
                else
                {
                    //Get bucket data, assume x increased with fixed interval
                    start = (int)(Math.Floor((i + 1) * bucket_size) + 1);   //Start index of next bucket
                    end = (int)(Math.Floor((i + 2) * bucket_size) + 1);     //End index of next bucket
                }

                if (end >= array.Length)    //Prevent index overrun
                {
                    end = array.Length;
                }

                int span = end - start; //Span = number of data points in next bucket
                //Debug.WriteLine("Bucket " + i + ": " + start + ", " + end + " : " + span);

                if (span <= 0) continue; //Skip empty bucket
                else
                {
                    for (int m = start; m < end; m++)
                    {
                        avg_x += m;
                        avg_y += array[m].Y;
                    }
                    avg_x /= span; //Average X Value
                    avg_y /= span; //Average Y Value

                    // Point a (Reference data point of previous bucket)
                    double a_x = a;
                    double a_y = array[a].Y;

                    double max_area = double.MinValue;
                    for (int n = bucket_start; n < bucket_end; n++)
                    {
                        // Calculate triangle area over three buckets
                        // Optimization - Rectange and Triangle area return same result, removed multiplication of 0.5
                        double area = Math.Abs((a_x - avg_x) * (array[n].Y - a_y) - (a_x - n) * (avg_y - a_y));
                        if (area > max_area)
                        {
                            max_area = area;
                            max_area_point = array[n];
                            next_a = n; // Next a is this b
                        }
                    }
                    // Pick this point from the Bucket
                    window[w++] = max_area_point;
                }

                // Current a becomes the next_a (chosen b)
                a = next_a;

                //Move on to next bucket
                bucket_start = start;
                bucket_end = end;
                start = end;
                if (end >= array.Length) break;
            }

            window[w++] = array[array.Length - 1]; // Always add last
            if (w < length)
            {
                //Resize array in case total length is less
                Array.Resize(ref window, w);
                Debug.WriteLine("Actual window size = " + w);
            }
            return window;
        }
    }
}
