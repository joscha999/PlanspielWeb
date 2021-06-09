using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PlanspielWeb.TagHelpers {
    [HtmlTargetElement("linechart")]
    public class LineChartTagHelper : TagHelper {
        public string ChartName { get; set; }
        public string ChartLabel { get; set; }
        public List<string> XLabels { get; set; }
        public List<float> YValues { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            output.TagName = "LineChartHelper";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Content.SetHtmlContent(GetScript(ChartName, ChartLabel, XLabels, YValues));
        }

        private static string GetScript(string chartName, string chartLabel, List<string> xLabels, List<float> yValues) {
            return @"<script type=""text/javascript"">
$(function () {
    var chartName = """ + chartName + @""";
    var ctx = document.getElementById(chartName).getContext('2d');
    var data = {
        labels: " + JsonConvert.SerializeObject(xLabels) + @",
        datasets: [{
            label: """ + chartLabel + @""",
            backgroundColor: [
                'rgba(255, 99, 132, 0.2)',
                'rgba(54, 162, 235, 0.2)',
                'rgba(255, 206, 86, 0.2)',
                'rgba(75, 192, 192, 0.2)',
                'rgba(153, 102, 255, 0.2)',
                'rgba(255, 159, 64, 0.2)',
                'rgba(255, 0, 0)',
                'rgba(0, 255, 0)',
                'rgba(0, 0, 255)',
                'rgba(192, 192, 192)',
                'rgba(255, 255, 0)',
                'rgba(255, 0, 255)'
            ],
            borderColor: [
                'rgba(255,99,132,1)',
                'rgba(54, 162, 235, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(75, 192, 192, 1)',
                'rgba(153, 102, 255, 1)',
                'rgba(255, 159, 64, 1)',
                'rgba(255, 0, 0)',
                'rgba(0, 255, 0)',
                'rgba(0, 0, 255)',
                'rgba(192, 192, 192)',
                'rgba(255, 255, 0)',
                'rgba(255, 0, 255)'
            ],
            borderWidth: 1,
            data: " + JsonConvert.SerializeObject(yValues) + @"
        }]
    };

    var options = {
        maintainAspectRatio: false,
        scales: {
            yAxes: [{
                ticks: {
                    beginAtZero: true
                },
                gridLines: {
                    display: true,
                    color: ""rgba(255,99,164,0.2)""
                }
            }],
            xAxes: [{
                ticks: {
                    min: 0,
                    beginAtZero: true
                },
                gridLines: {
                    display: false
                }
            }]
        }
    };

    var myChart = new Chart(ctx, {
        options: options,
        data: data,
        type: 'line'
    });
});
</script>";
        }
    }
}
