$(document).ready(function(){
         
         // New Clients Chart
         
         
         var data = {
         labels: ['7th','7th', '6th', '5th', '4th', '3rd', '2nd', '1st','7th', '6th', '5th', '4th', '3rd', '2nd', '1st'],
         series: [
           [5, 4, 3, 7, 5, 10, 3, 4, 3, 7, 5, 10, 3, 4, 3]
         
         ]
         };
         
         var options = {
         axisX: {
           offset: 0,
           labelOffset: {
             x: -20,
             y: 0
           },
           showLabel: false,
           showGrid: false,
          
           labelInterpolationFnc: Chartist.noop,
           
         },
         
         axisY: {
          
           offset: 0,
          
           position: "start",
           
           labelOffset: {
             x: 0,
             y: 0
           },
          
           showLabel: true,
           showGrid: false,
           labelInterpolationFnc: Chartist.noop,
           type: undefined,
          scaleMinSpace: 20,
          
         },
         plugins: [Chartist.plugins.tooltip({ class: "newclints", appendToBody: true })]
         };
         
         // All you need to do is pass your configuration as third parameter to the chart function
         new Chartist.Bar("#newclints", data, options);
         // New Clients Chart
         
         
         var data = {
         labels: ['7th','7th', '6th', '5th', '4th', '3rd', '2nd', '1st','7th', '6th', '5th', '4th', '3rd', '2nd', '1st'],
         series: [
           [5, 4, 3, 7, 5, 10, 3, 4, 3, 7, 5, 10, 3, 4, 3]
         
         ]
         };
         
         var options = {
         axisX: {
           offset: 0,
           labelOffset: {
             x: -20,
             y: 0
           },
           showLabel: false,
           showGrid: false,
          
           labelInterpolationFnc: Chartist.noop,
           
         },
         
         axisY: {
          
           offset: 0,
          
           position: "start",
           
           labelOffset: {
             x: 0,
             y: 0
           },
          
           showLabel: true,
           showGrid: false,
           labelInterpolationFnc: Chartist.noop,
           type: undefined,
          scaleMinSpace: 20,
          
         }
         ,
         plugins: [Chartist.plugins.tooltip({ class: "newclints", appendToBody: true })]
         };
         
         // All you need to do is pass your configuration as third parameter to the chart function
         new Chartist.Bar("#totalsales", data, options);
         
         
         
         var data = {
         labels: ['7th','7th', '6th', '5th', '4th', '3rd', '2nd', '1st','7th', '6th', '5th', '4th', '3rd', '2nd', '1st'],
         series: [
           [5, 4, 3, 7, 5, 10, 3, 4, 3, 7, 5, 10, 3, 4, 3]
         
         ]
         };
         
         var options = {
         axisX: {
           offset: 0,
           labelOffset: {
             x: -20,
             y: 0
           },
           showLabel: false,
           showGrid: false,
          
           labelInterpolationFnc: Chartist.noop,
           
         },
         
         axisY: {
          
           offset: 0,
          
           position: "start",
           
           labelOffset: {
             x: 0,
             y: 0
           },
          
           showLabel: true,
           showGrid: false,
           labelInterpolationFnc: Chartist.noop,
           type: undefined,
          scaleMinSpace: 20,
          
         }
         ,
         plugins: [Chartist.plugins.tooltip({ class: "newclints", appendToBody: true })]
         };
         
         // All you need to do is pass your configuration as third parameter to the chart function
         new Chartist.Bar("#profit", data, options);
         
         
         var data = {
         labels: ['7th','7th', '6th', '5th', '4th', '3rd', '2nd', '1st','7th', '6th', '5th', '4th', '3rd', '2nd', '1st'],
         series: [
           [5, 4, 3, 7, 5, 10, 3, 4, 3, 7, 5, 10, 3, 4, 3]
         
         ]
         };
         
         var options = {
         axisX: {
           offset: 0,
           labelOffset: {
             x: -20,
             y: 0
           },
           showLabel: false,
           showGrid: false,
          
           labelInterpolationFnc: Chartist.noop,
           
         },
         
         axisY: {
          
           offset: 0,
          
           position: "start",
           
           labelOffset: {
             x: 0,
             y: 0
           },
          
           showLabel: true,
           showGrid: false,
           labelInterpolationFnc: Chartist.noop,
           type: undefined,
          scaleMinSpace: 20,
          
         }
         ,
         plugins: [Chartist.plugins.tooltip({ class: "newclints", appendToBody: true })]
         };
         
         // All you need to do is pass your configuration as third parameter to the chart function
         new Chartist.Bar("#newinvoices", data, options);
         
         
         
         
         
         
         
         new Chartist.Line('#line-chart', {
         labels: ['1st Aug', '2nd Aug', '3rd Aug', '4th Aug', '5th Aug', '6th Aug', '7th Aug'],
         
         series: [
               {meta:"DOT NET", data:  [20, 40, 33, 67, 93, 67, 47]},
               {meta:"PHP ", data:  [43, 56, 76, 66, 66, 43,78] }
          ]
         }, {
         high:100,
         low: 0,
         fullWidth: true,
        
         // As this is axis specific we need to tell Chartist to use whole numbers only on the concerned axis
         axisY: {
           onlyInteger: true,
           offset: 20,
         
         },
          axisX: {
           
           showLabel: true,
           showGrid: false,
          labelOffset: {
             x: -20,
             y: 0
           },
           
         },
          plugins: [
          Chartist.plugins.tooltip({ class: "line-chart", appendToBody: true }),
         // Chartist.plugins.legend({
         //   legendNames: ['Blue pill', 'Red pill', 'Purple pill'],
         //  })
          ],
         
         
         });
         
         
         
         new Chartist.Line('#fill-line-chart', {
         labels: ['1st Aug', '2nd Aug', '3rd Aug', '4th Aug', '5th Aug', '6th Aug', '7th Aug'],
         
         series: [
               {meta:"DOT NET", data:  [20, 40, 33, 67, 93, 67, 47]},
               {meta:"PHP ", data:  [43, 56, 76, 66, 66, 43,78] }
          ]
         }, {
         high:100,
         low: 0,
         fullWidth: true,
         // As this is axis specific we need to tell Chartist to use whole numbers only on the concerned axis
         axisY: {
           onlyInteger: true,
           offset: 20,
         showGrid: false,
         },
          axisX: {
           
           showLabel: true,
           showGrid: false,
          labelOffset: {
             x: -20,
             y: 0
           },
           
         },
          plugins: [
          Chartist.plugins.tooltip({ class: "line-chart", appendToBody: true }),
         // Chartist.plugins.legend({
         //   legendNames: ['Blue pill', 'Red pill', 'Purple pill'],
         //  })
          ],
         
         
         });
         
         new Chartist.Bar('#bar-chart', {
         labels: ['1st Aug', '2nd Aug', '3rd Aug', '4th Aug', '5th Aug', '6th Aug', '7th Aug'],
         
         series: [
               {meta:"DOT NET", data:  [20, 40, 33, 67, 93, 67, 47]},
               {meta:"PHP ", data:  [43, 56, 76, 66, 66, 43,78] }
          ]
         }, {
         high:100,
         low: 0,
         fullWidth: true,
         // As this is axis specific we need to tell Chartist to use whole numbers only on the concerned axis
         axisY: {
           onlyInteger: true,
           offset: 20,
         
         
         },
          axisX: {
           
           showLabel: true,
           showGrid: false,
          labelOffset: {
             x: -20,
             y: 0
           },
           
         },
          plugins: [
          Chartist.plugins.tooltip({ class: "line-chart", appendToBody: true }),
         // Chartist.plugins.legend({
         //   legendNames: ['Blue pill', 'Red pill', 'Purple pill'],
         //  })
          ],
         
         
         });
         
         new Chartist.Bar('#fill-bar-chart', {
         labels: ['1st Aug', '2nd Aug', '3rd Aug', '4th Aug', '5th Aug', '6th Aug', '7th Aug'],
         
         series: [
               {meta:"DOT NET", data:  [20, 40, 33, 67, 93, 67, 47]},
               {meta:"PHP ", data:  [43, 56, 76, 66, 66, 43,78] }
          ]
         }, {
         high:100,
         low: 0,
         fullWidth: true,
         // As this is axis specific we need to tell Chartist to use whole numbers only on the concerned axis
         axisY: {
           onlyInteger: true,
           offset: 20,
            showGrid: false,
         
         },
          axisX: {
           
           showLabel: true,
           showGrid: false,
          labelOffset: {
             x: -20,
             y: 0
           },
           
         },
          plugins: [
          Chartist.plugins.tooltip({ class: "line-chart", appendToBody: true }),
         // Chartist.plugins.legend({
         //   legendNames: ['Blue pill', 'Red pill', 'Purple pill'],
         //  })
          ],
         
         
         });
         
         
         new Chartist.Pie('#pie-chart', {
         series: [20, 10, 30, 40]
         }, {
         donut: true,
         donutWidth: 60,
         donutSolid: true,
         startAngle: 270,
         showLabel: true
         });
         
         new Chartist.Line('#polar-chart', {
         labels: [1, 2, 3, 4, 5, 6, 7, 8],
         series: [
           [50, 44, 65, 33, 93, 23, 27, 55],
           [54, 57, 87, 39, 12, 28, 77, 67],
          
          
         ]
         }, {
        
         showArea: true,
         showLine: false,
         showPoint: false,
         fullWidth: true,
         axisX: {
           showLabel: true,
           showGrid: false
         },
         axisY: {
            offset:20,
         }
         });
         
         })