using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PlanspielWeb.TagHelpers {
    [HtmlTargetElement("chart")]
    public class ChartTagHelper : TagHelper {
        public string ChartType { get; set; }
        public string ChartName { get; set; }
        public string ChartLabel { get; set; }
        public List<string> XLabels { get; set; }
        public List<float> YValues { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            output.TagName = "LineChartHelper";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Content.SetHtmlContent(GetScript(ChartType, ChartName, ChartLabel, XLabels, YValues));
        }

        private static string GetScript(string chartType, string chartName,
            string chartLabel, List<string> xLabels, List<float> yValues) {
            return @"<script type=""text/javascript"">
$(function () {
    var chartName = """ + chartName + @""";
    var ctx = document.getElementById(chartName).getContext('2d');
    var data = {
        labels: " + JsonConvert.SerializeObject(xLabels) + @",
        datasets: [{
            label: """ + chartLabel + @""",
            backgroundColor: [
                'rgba(230, 25, 75, 0.2)',
                'rgba(60, 180, 75, 0.2)',
                'rgba(255, 225, 25, 0.2)',
                'rgba(0, 130, 200, 0.2)',
                'rgba(245, 130, 48, 0.2)',
                'rgba(70, 240, 240, 0.2)',
                'rgba(240, 50, 230, 0.2)',
                'rgba(250, 190, 212, 0.2)',
                'rgba(0, 128, 128, 0.2)',
                'rgba(220, 190, 255, 0.2)',
                'rgba(170, 110, 40, 0.2)',
                'rgba(255, 250, 200, 0.2)',
                'rgba(128, 0, 0, 0.2)',
                'rgba(170, 255, 195, 0.2)',
                'rgba(0, 0, 128, 0.2)',
                'rgba(128, 128, 128, 0.2)',
                'rgba(255, 255, 255, 0.2)',
                'rgba(0, 0, 0, 0.2)'
            ],
            borderColor: [
                'rgba(230, 25, 75, 1)',
                'rgba(60, 180, 75, 1)',
                'rgba(255, 225, 25, 1)',
                'rgba(0, 130, 200, 1)',
                'rgba(245, 130, 48, 1)',
                'rgba(70, 240, 240, 1)',
                'rgba(240, 50, 230, 1)',
                'rgba(250, 190, 212, 1)',
                'rgba(0, 128, 128, 1)',
                'rgba(220, 190, 255, 1)',
                'rgba(170, 110, 40, 1)',
                'rgba(255, 250, 200, 1)',
                'rgba(128, 0, 0, 1)',
                'rgba(170, 255, 195, 1)',
                'rgba(0, 0, 128, 1)',
                'rgba(128, 128, 128, 1)',
                'rgba(255, 255, 255, 1)',
                'rgba(0, 0, 0, 1)'
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
        type: '" + chartType + @"'
    });
});
</script>";
        }
    }
}
