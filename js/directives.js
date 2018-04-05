'use strict';
'use strict';
/**
	Custom Directives.
*/
angular.module('mhmApp.directives', [])
.directive('appVersion', ['version', function(version) {
return function(scope, elm, attrs) {
  elm.text(version);
};
}])
.directive('myRefresh',function($location,$route){
    return function(scope, element, attrs) {
        element.bind('click',function(){
            if(element[0] && element[0].href && element[0].href === $location.absUrl()){
                $route.reload();
            }
        });
    }   
})
.directive('numericOnly', function(){
    return {
        require: 'ngModel',
        link: function(scope, element, attrs, modelCtrl) {				
            modelCtrl.$parsers.push(function (inputValue) {
                var transformedInput = inputValue ? inputValue.replace(/[^\d.-]/g,'') : null;
				if (transformedInput!=inputValue) {
                    modelCtrl.$setViewValue(transformedInput);
                    modelCtrl.$render();
                }

                return transformedInput;
            });
        }
    };
})
.directive('intOnly', function(){
    return {
        require: 'ngModel',
        link: function(scope, element, attrs, modelCtrl) {

            modelCtrl.$parsers.push(function (inputValue) {
                var transformedInput = inputValue ? inputValue.replace(/[^\d]/g,'') : null;

                if (transformedInput!=inputValue) {
                    modelCtrl.$setViewValue(transformedInput);
                    modelCtrl.$render();
                }

                return transformedInput;
            });
        }
    };
})
.directive('ngMax', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, elem, attr, ctrl) {
            scope.$watch(attr.ngMax, function () {
                ctrl.$setViewValue(ctrl.$viewValue);
            });
            var maxValidator = function (value) {
                var max = attr.max || Infinity;				
                if (!isEmpty(value) && value > max) {
                    ctrl.$setValidity('ngMax', false);
                    return undefined;
                } else {
                    ctrl.$setValidity('ngMax', true);
                    return value;
                }
            };

            ctrl.$parsers.push(maxValidator);
            ctrl.$formatters.push(maxValidator);
        }
    };
})
.directive('dateInput', function($filter, $browser) {
    return {
        require: 'ngModel',
        link: function($scope, $element, $attrs, ngModelCtrl) {
            var listener = function() {
                var value = $element.val().replace(/[^0-9]/g, '');
                $element.val($filter('dateB')(value, false));
            };
            // This runs when we update the text field
            ngModelCtrl.$parsers.push(function(viewValue) {
               return viewValue;
            });
            // This runs when the model gets updated on the scope directly and keeps our view in sync
            ngModelCtrl.$render = function() {
                $element.val($filter('dateB')(ngModelCtrl.$viewValue, false));
            };
            $element.bind('change', listener);
            $element.bind('keydown', function(event) {
                var key = event.keyCode;
                // If the keys include the CTRL, SHIFT, ALT, or META keys, or the arrow keys, do nothing.
                // This lets us support copy and paste too
                if (key == 91 || (15 < key && key < 19) || (37 <= key && key <= 40)){
                    return;
                }
                $browser.defer(listener); // Have to do this or changes don't get picked up properly
            });
            $element.bind('paste cut', function() {
                $browser.defer(listener);
            });
        }

    };
})
.directive('phoneInput', function($filter, $browser) {
    return {
        require: 'ngModel',
        link: function($scope, $element, $attrs, ngModelCtrl) {
            var listener = function() {
                var value = $element.val().replace(/[^0-9]/g, '');
                $element.val($filter('tel')(value, false));
            };
            // This runs when we update the text field
            ngModelCtrl.$parsers.push(function(viewValue) {
                return viewValue.replace(/[^0-9]/g, '').slice(0,10);
            });
            // This runs when the model gets updated on the scope directly and keeps our view in sync
            ngModelCtrl.$render = function() {
                $element.val($filter('tel')(ngModelCtrl.$viewValue, false));
            };

            $element.bind('change', listener);
            $element.bind('keydown', function(event) {
                var key = event.keyCode;
                // If the keys include the CTRL, SHIFT, ALT, or META keys, or the arrow keys, do nothing.
                // This lets us support copy and paste too
                if (key == 91 || (15 < key && key < 19) || (37 <= key && key <= 40)){
                    return;
                }
                $browser.defer(listener); // Have to do this or changes don't get picked up properly
            });
            $element.bind('paste cut', function() {
                $browser.defer(listener);
            });
        }

    };
})
.directive('formatCurrency', function ($filter, $browser) {
	return {
        require: 'ngModel',
        link: function($scope, $element, $attrs, ngModelCtrl) {
            var listener = function() {
                var value = $element.val().replace(/[^0-9]/g, '');
                $element.val($filter('inputcurrency')(value, false));
            };
            // This runs when we update the text field
            ngModelCtrl.$parsers.push(function(viewValue) {
                return viewValue;
            });

            // This runs when the model gets updated on the scope directly and keeps our view in sync
            ngModelCtrl.$render = function() {
                $element.val($filter('inputcurrency')(ngModelCtrl.$viewValue, false));
            };

            $element.bind('change', listener);
            $element.bind('keydown', function(event) {
                var key = event.keyCode;
                // If the keys include the CTRL, SHIFT, ALT, or META keys, or the arrow keys, do nothing.
                // This lets us support copy and paste too
                if (key == 91 || (15 < key && key < 19) || (37 <= key && key <= 40)){
                    return;
                }
                $browser.defer(listener); // Have to do this or changes don't get picked up properly
            });

            $element.bind('paste cut', function() {
                $browser.defer(listener);
            });
        }

    };
})
.directive('d3Bars', ['$window', '$timeout', 'CaseService','$rootScope' ,
  function($window, $timeout, CaseService,$rootScope) {
    return {
      //restrict: 'EA',
      scope: {
			data: '=', // bi-directional data-binding	
			width: '=width',
			height: '=height',
	  }, 
      link: function(scope, ele, attrs) {		
        CaseService.d3service.d3().then(function(d3) {		
		var renderTimeout;        
		$rootScope.updateGraph = function(area,flag,callback){				
			if(typeof scope.data!='undefined' && typeof scope.data.post!='undefined' && scope.data.post.length>0){		
				scope.render(area,angular.copy(angular.copy(scope.data)),flag,callback);
								
			}
		};			  
	
		 
         scope.$watch('data', function(newData) {	
	 
			d3.select(ele[0]).html('');
			d3.select(ele[0]).select("svg").remove();	
			$(ele[0]).find('svg').html('');	
			
			if(typeof scope.data!='undefined' && typeof scope.data.post!='undefined' && scope.data.post.length>0){		
					if(scope.data.totalRecord==scope.data.post.length){						
						scope.render(ele,angular.copy(scope.data),false);
					}				
				}
          }, true);	 
			
          scope.render = function(ele,data,flag,callback) {			
			d3.select(ele[0]).html('');
			d3.select(ele[0]).select("svg").remove();		
			
								  
			var margin = {top: 140, right: 7.5, bottom: 100, left: 25};
			var height=scope.height;
			var width=data.width;  
			//var width=788;
			var tickSize=8;
			var length=scope.data.post.length;
			var length=data.post.length;
			
			if(flag){
				var margin =  {top: 150, right: 3, bottom: 100, left: 52};
				height=550;		
				width=795;
			}			

			
			
			if(flag){
				width =  width - (margin.left*4) - (margin.right*4);	
			}
			else
			{
				width =  width - (margin.left*length) - (margin.right*length);	
			}
			
			height = height - margin.top - margin.bottom;
			
			function wrap(text, width) {					
				  text.each(function() {
					var text = d3.select(this),
					tex=text.text(),
					
						words = text.text().split(/\s+/).reverse(),
						word,
						line = [],
						lineNumber = 0,
						lineHeight = 1, // ems
						y = text.attr("y"),
						dy = parseFloat(text.attr("dy"))+1,
						tspan = text.text(null).append("tspan").attr("x", 0).attr("y", y).attr("dy", dy + "em");	
									
					while (word = words.pop()) {
						word=word.toString().split("-")[0];
						
					  line.push(word);
					  tspan.text(line.join(" "));							
					  if (tspan.node().getComputedTextLength() > width) {
						line.pop();
						tspan.text(line.join(" "));
						line = [word];
						tspan = text.append("tspan").attr("x", 0).attr("y", y).attr("dy", ++lineNumber * lineHeight + dy + "em").text(word);
					  }
					}
				  });
				}			
				
			function wrapNEW(text, width) {					
				  text.each(function() {
					 
					var text = d3.select(this),
					tex=text.text(),
					words = text.text().split(/\s+/).reverse(),
					word,
					line = [],
					lineNumber = 0,
					lineHeight = .7, // ems
					y = text.attr("y"),
					x = text.attr("x"),
					dy = parseFloat(text.attr("dy"))-0.10;
					
					if(words.length >4)
					{
					var tspan = text.text(null).append("tspan").attr("x", x).attr("y", y).attr("dy", "-0.2em");	
					}
					else
					{
					var tspan = text.text(null).append("tspan").attr("x", x).attr("y", y).attr("dy", dy + "em");	
						
					}
						
					 //console.log('wrap'+words)				
					while (word = words.pop()) {
						
						//console.log('word'+word);
					  line.push(word);
					  tspan.text(line.join(" "));							
					  if (tspan.node().getComputedTextLength() > width) {
						dy = 0;  
						line.pop();
						tspan.text(line.join(" "));
						line = [word];
						tspan = text.append("tspan").attr("x", x).attr("y", y).attr("dy", ++lineNumber * lineHeight + dy + "em").text(word);
					  }
					}
				  });
				}	
				
            if (!data.post) return;			
            if (renderTimeout) clearTimeout(renderTimeout);			
            renderTimeout = $timeout(function() { 		
					var xRange=.3;
					var fontSizeRect='9px';
					if(flag){
						xRange=.3;
						width;
						fontSizeRect='15px';
					}
					var x = d3.scale.ordinal()
						 .rangeRoundBands([0, width],xRange, .2);
					
					var y = d3.scale.linear()
						.rangeRound([height, 0]);
					
					var color = d3.scale.ordinal()
						.range(["#FDE796","#C7E6A3"]);
						
					var xAxis = d3.svg.axis()
						.scale(x)
						.orient("bottom");
						//.orient("bottom").ticks(5).tickSize(-height, 0, 0);

					var yAxis = d3.svg.axis()
						.scale(y)
						.orient("left")
						 .tickFormat(d3.format("$0,000")).ticks(tickSize).tickSize(-width, 0, 0);
					var fmt = yAxis.tickFormat();	

					var translateValue=90;
					var adjustedWidth=340;
					if(flag){
						translateValue=80;
						adjustedWidth=380;
					}
					
					
					var svg = d3.select(ele[0]).append('svg')
						.attr("width", width+adjustedWidth)
						.attr("height", height + margin.top + margin.bottom)
						.append("g")
						.attr("transform", "translate(" + translateValue + "," + margin.top + ")");

						
						
					  color.domain(d3.keys(data.post[0]).filter(function(key) { return key !== "State" && key !== "Rank"; }));
					  
					  data.post.forEach(function(d) {
						//console.log(d);
						var y0 = 0;
						var old=0;
						var old1 =0;
						d.ages = color.domain().map(function(name,i) {	
							y1=y0+d[name];	
							//console.log(y1);
							if(d[name] < 0){			
								if(i == 0){	
									old1 = d[name];
									return {name: name, y0: 0, y1: d[name],y2:d[name]};
								}else{
									return {name: name, y0: old1, y1: d[name]+old1,y2:d[name]};
								}
							}else{
								if(i == 0){
									old = d[name];
									return {name: name, y0: 0, y1: d[name],y2:d[name]};
								}else{
									return {name: name, y0: old, y1: old+d[name],y2:d[name]};
								}
							}	
						});
						d.total = d.ages[0].y2+d.ages[1].y2;
					  });					  
					  x.domain(data.post.map(function(d) { return d.State; }));
					 
					  var y1=d3.min(data.post, function(d) {  return d.ages[0].y1; });
					  var y2=d3.min(data.post, function(d) { return d.ages[1].y1; });
					  
					  var My1=d3.max(data.post, function(d) {  return d.ages[0].y1; });
					  var My2=d3.max(data.post, function(d) { return d.ages[1].y1; });

					  var min=d3.min([y1,y2]);
					  var max=d3.max([My1,My2]);

						 if(min>0){
							min=0;
						 }
						 
						if(max<0){
							var max=0;
						}

					  y.domain([min, max]);					
						if(flag){
							  svg.append("g")
							  .attr("class", "grid")
							  .attr("class", "x axis")
							  .attr("transform", "translate(0," + height + ")")	  
							  .call(xAxis)
							  .selectAll(".tick text")
							  .call(wrap, 75);  
						}else{
							 svg.append("g")
							  .attr("class", "grid")
							  .attr("class", "x axis")
							  .attr("transform", "translate(0," + height + ")")	  
							  .call(xAxis)
							  .selectAll(".tick text")
							  .call(wrap, 75); 
						}		  
						 
					  svg.append("g")
						  .attr("class", "grid")
						  .attr("class", "y axis")
						 
						 .call(yAxis)	  
						.append("text")
						  .attr("transform", "rotate(-90)")
						  .attr("y", 6)
						  .attr("dy", ".71em")
						  .style("text-anchor", "end")						  
						  .text("");							
						var tooltip = d3.select('body').append('div')
							.style('position','absolute') 
							.style('padding','0 10px') 
							.style('opacity',0)  
					   
						svg.selectAll('.axis line, .axis path')
						 .style({'stroke': '#ccc', 'fill': 'none', 'stroke-width': '1px'});
						 
					  var state = svg.selectAll(".state")
						  .data(data.post)
						  .enter().append("g")
						  .attr("class", "g")	 
						  .attr("transform", function(d) { return "translate(" + x(d.State) + ",0)"; });


						  state.selectAll("rect")
						  .data(function(d) { return d.ages; })
						  .enter().append("rect")
						  .attr("width", x.rangeBand())
						  .attr("y", function(d,i) {			
							  var y01=y(d.y0) - y(d.y1);
							  return (y01<0)?(y(d.y1)-(Math.abs((y(d.y0) - y(d.y1))))):y(d.y1);
						   })
						   .attr("height", function(d) { 			
								return Math.abs(y(d.y0) - y(d.y1));
							})
						  .style("stroke", function(d) { return 'rgb(3,36,159)'; })
							.style("stroke-width", function(d) { return '0.3px'; })		
						  .style("fill", function(d) { return color(d.name); })
							//To change the color to yellow on mouse over and to set the opactiy to 0.5
							.on('mouseover',function(d){
							var txt='<div style="padding: 5px 10px; border: 1px solid rgb(81, 81, 81); color: rgb(255, 255, 255); border-radius: 5px; font-size: 10px; font-weight: bold;background: none 0px 0px repeat scroll rgba(0, 0, 0, 0.6);"><span style="background:'+color(d.name)+' none repeat scroll 0% 0%; float: left; width: 15px; height: 15px; margin-right: 5px;"></span>';		
							txt +=d.name+': '+fmt(d.y2);
							txt +='</div>';		
							tooltip.transition()
							.style('opacity',.9) 
							tooltip.html(txt)
							.style('left',(d3.event.pageX - 20)+ 'px') //position of the tooltip
							.style('top',(d3.event.pageY + 15) + 'px') 
							d3.select(this).style('opacity',.7)
							})
							//To reset the color, hence opacity = 1
							.on('mouseout',function(d){
								tooltip.html('')
								d3.select(this).style('opacity',1)									
							});						
						
						 state.selectAll("text")
						  .data(function(d) { return d.ages; })
						  .enter().append("text")        
						  .attr("y", function(d) { return y(d.y1)+((y(d.y0) - y(d.y1))/2); })
						  .attr("x", function(d) { return (x.rangeBand()/2); })
						  .attr("height", function(d) { return y(d.y0) - y(d.y1); })
						  .attr("text-anchor","middle")  
						  .attr("alignment-baseline","central")
						  .attr("class", "textVal")	
						  .style({'fill':function(d) { return '#000'; }})
						  .text(function(d){								
							  return fmt(d.y2); 	  
						   })
						   //To change the color to yellow on mouse over and to set the opactiy to 0.5
							.on('mouseover',function(d){
							var txt='<div style="padding: 5px 10px; border: 1px solid rgb(81, 81, 81); color: rgb(255, 255, 255); border-radius: 5px; font-size: 10px; font-weight: bold;background: none 0px 0px repeat scroll rgba(0, 0, 0, 0.6);"><span style="background:'+color(d.name)+' none repeat scroll 0% 0%; float: left; width: 15px; height: 15px; margin-right: 5px;"></span>';		
							txt +=d.name+': '+fmt(d.y2);
							txt +='</div>';		
							tooltip.transition()
							.style('opacity',.9) 
							tooltip.html(txt)
							.style('left',(d3.event.pageX - 20)+ 'px') //position of the tooltip
							.style('top',(d3.event.pageY + 15) + 'px') 			 
						
							d3.select(this).style('opacity',.7)})
							//To reset the color, hence opacity = 1
							.on('mouseout',function(d){
								tooltip.html('')
								d3.select(this)
									.style('opacity',1)									
							});						   
						 
						 var j=1;						 
						 var text=svg.selectAll(".g")         
						  .append("text")	 
						  .attr("class", "Endtext")	  
						  .attr("x", function(d) { return (x.rangeBand()/2); })
						  .attr("y", function(d) { 
								var slot=(max/tickSize)/2 ;									
								if(d.ages[1].y2 < d.ages[0].y2){		
									if(d.total<d.ages[0].y2){
										if(d.ages[0].y2<0){
											
											return y(slot);
										}else{
											slot=(d.ages[0].y2==0 || d.ages[1].y2==0)?(slot*3):slot;
											// console.log("Top Test ");
											// console.log(d.ages[0].y2+slot);
											return y(d.ages[0].y2+slot);
										}
										
									}else{
										if(d.ages[0].y2<0){
											
											return y(slot);
											
										}else{
											slot=(d.ages[0].y2==0 || d.ages[1].y2==0)?(slot*3):slot;
											// console.log("Top Test ");
											// console.log(d.total+slot);
											return y(d.total+slot);
										}
										
									}			
								}else{
									if(d.total<d.ages[1].y2){
										if(d.ages[0].y2<0){
											console.log(0);
											return y(slot);
										}else{
											slot=(d.ages[0].y2==0 || d.ages[1].y2==0)?(slot*3):slot;
											
											return y(d.ages[1].y2+slot);
										}
										
									}else{
										if(d.ages[0].y2<0){
											
											return y(slot);
										}else{
											slot=(d.ages[0].y2==0 || d.ages[1].y2==0)?(slot*3):slot;
											
											return y(d.total+slot);
										}				
									}
									
								}
							})
						  .attr("text-anchor","middle")
						  .attr("class", "textVal1 ToptextVal")	
						  .text(function(d){
								j= j+1;
								return fmt((parseFloat(d.ages[0].y2)+parseFloat(d.ages[1].y2))); 
						  }).style({'fill':function(d) { return 'rgb(3,36,159)'; }});
						  
						  
						  
						/*Create the circle for each block */	
						var circle = svg.selectAll(".g").append("circle")
							.attr("r", "26" )
							.attr("class","circletop") 
							.attr("cx", function(d) { return (x.rangeBand()/2); })
							.attr("stroke","#f47421")
							.style("fill", '#f47421')
							.attr("transform", "translate(40," + -12 + ")")	
							.attr("fill", "white");				
						  
						  var i=0;
						  
						  svg.selectAll(".g")         
						  .append("text")	 
						  .attr("class", "Circletext")	 
						  .attr("x", function(d) { return (x.rangeBand()/2); })
						  .attr("y", function(d) { 
								var slot=(max/tickSize)/2  ;									
								if(d.ages[1].y2 < d.ages[0].y2){		
									if(d.total<d.ages[0].y2){
										if(d.ages[0].y2<0){
											return y(slot);
										}else{
											slot=(d.ages[0].y2==0 || d.ages[1].y2==0)?(slot*3):slot;
											return y(d.ages[0].y2+slot);
										}
										
									}else{
										if(d.ages[0].y2<0){
											return y(slot);
										}else{
											slot=(d.ages[0].y2==0 || d.ages[1].y2==0)?(slot*3):slot;
											return y(d.total+slot);
										}
										
									}			
								}else{
									if(d.total<d.ages[1].y2){
										if(d.ages[0].y2<0){
											return y(slot);
										}else{
											slot=(d.ages[0].y2==0 || d.ages[1].y2==0)?(slot*3):slot;
											return y(d.ages[1].y2+slot);
										}
										
									}else{
										if(d.ages[0].y2<0){
											return y(slot);
										}else{
											slot=(d.ages[0].y2==0 || d.ages[1].y2==0)?(slot*3):slot;
											return y(d.total+slot);
										}				
									}
									
								}
							})
						  .attr("text-anchor","middle")
						  .attr("class", "circletextVal textVal2")	
						  .text(function(d){
								return "#" + ++i; }).style({'fill':function(d) { return 'rgb(255,255,255)'; }});
						  
						  
						  //font-size: 16px;
						var rectLegend=230;
						var fontSize='13px';
						var fontFamily='helvetica';
						var fontSizeX='16px';
						var textValL='14px';
						var textBold='bold';
						
						var legends=[];
						// legends.push("YOUR TOTAL PAYMENTS");
						legends.push(color.domain().slice()[1]);
						legends.push(color.domain().slice()[0]);
						// legends.push("YOUR TOTAL PAYMENTS");
										
						var legendsColor = d3.scale.ordinal()
						.range(["#C7E6A3","#FDE796"]);
										
						if(flag){
							 fontSize='13px';
							 fontFamily='calibri';
							 fontSizeX='16px';
							 textValL='14px';
							 rectLegend=240;
							 textBold='bold';
							 
							 var legend = svg.selectAll(".legend")
							   .data(legends)
							  .enter().append("g")
							   .attr("class", "legend")
							   .attr("transform", function(d, i) { return "translate(0," + (parseInt(i * 50)-50) + ")"});			
							    legend.append("rect")
								   .attr("x", 655)
								   .attr("y", rectLegend+110-65)
								   .attr("width", 185)
								   .attr("height", 28)
								   .attr("rx", 5)
								   .attr("ry", 5)
								   .style("fill", legendsColor);

							   legend.append("text")
								   .attr("x", 745)
								   .attr("y", rectLegend+110-50)
								   .attr("dy", ".45em") 
								   .attr("text-anchor","middle")
								  								  
								   .text(function(d) { return d; })
								   .style("font-size", "16px")
								   .style({'font-weight': 'bold'})
								   .call(wrapNEW, 180); 
								  
								
								  
						}else{
							 
							var legend = svg.selectAll(".legend")
							  .data(legends)
							  .enter().append("g")
							  .attr("class", "legend")
							  
							  //.attr("transform", function(d, i) { return "translate(0," + ((parseInt(i * 20))-15) + ")"});
							  .attr("transform", function(d, i) { return "translate(0," + ((parseInt(i * 40))-40) + ")"});
							  
							   legend.append("rect")
							  .attr("x", width+adjustedWidth-(317+5*(5-length)))
							  .attr("y", rectLegend-55)
							  .attr("width", 185)
							  .attr("height", 28)
							  .attr("rx", 5)
							  .attr("ry", 5)
							  .style("fill", legendsColor)
							  
						       legend.append("text")
							  .attr("x", width+adjustedWidth-(225+5*(5-length)) )
							  .attr("y", rectLegend-40)
							  .attr("dy", ".35em")
							  .attr("text-anchor","middle")
							  							  
							  .text(function(d) { return d; })
							  .style("font-size", "16px")
							  .style({'font-weight': 'bold'})
							  .call(wrapNEW, 180); 
						}	

					
					
						svg.selectAll('text')
						.style({'font-size': fontSize,'font-family':fontFamily});					
						
						svg.selectAll('.axis').selectAll('text')
						.style({'font-size': '16px'}).style({'font-weight': textBold});
						
						svg.selectAll('.textVal')
						.style({'font-size': fontSizeX});
						
						svg.selectAll('.textVal')
						.style({'font-weight': textBold});
						
						svg.selectAll('.textValL')
						.style({'font-size': textValL});
						
						svg.selectAll('.textVal1')
						.style({'font-size': '18pt'});
						
						svg.selectAll('.textVal1')
						.style({'font-weight': textBold});
						
						svg.selectAll('.y.axis').selectAll('text')
						.style({'font-size': '12px'});
						
						svg.selectAll('.legend').selectAll('text')
						.style({'font-size': '14px'}).style({'font-weight': 'bold'});
						
						svg.selectAll('.textVal2')
						.style({'font-size': '28px'});
						
						if(flag){
							
						svg.append("text")
							  .attr("x", 745 )
							  .attr("y", 215)
							  .attr("dy", ".35em")
							  .attr("text-anchor","middle")
							  .text('YOUR TOTAL PAYMENTS')
							  .style("font-size", "12px")
							  .style("fill", "rgb(3, 36, 159)")
							  .style({'font-weight': 'bold'});
							  
							  svg.append("text")
							  .attr("x", 745)
							  .attr("y", 180)
							  .attr("dy", ".35em")
							  .attr("text-anchor","middle")
							  .text('Key to Graph')
							  .attr("text-decoration","underline")
							  .style("font-size", "18px")
							  .style({'font-weight': 'bold'});
						
						  svg.append("rect")
							  .attr("x", 648 )
							  .attr("y", 165)
							  .attr("width", 202)
							  .attr("height", 160)
							  .style("fill", "transparent")
							  .style("stroke", "black")
							  .style("stroke-width", 3)
							  .style("opacity", 0.5)
						}
						else{
							svg.append("text")
							  .attr("x", width+adjustedWidth-(225+5*(5-length)) )
							  .attr("y", 110)
							  .attr("dy", ".35em")
							  .attr("text-anchor","middle")
							  .text('YOUR TOTAL PAYMENTS')
							  .style("font-size", "14px")
							  .style("fill", "rgb(3, 36, 159)")
							  .style({'font-weight': 'bold'});
							  
							  svg.append("text")
							  .attr("x", width+adjustedWidth-(225+5*(5-length)) )
							  .attr("y", 80)
							  .attr("dy", ".35em")
							  .attr("text-anchor","middle")
							  .text('Key to Graph')
							  .attr("text-decoration","underline")
							  .style("font-size", "18px")
							  .style({'font-weight': 'bold'});
						
						svg.append("rect")
							  .attr("x", (164*length))
							  .attr("y", 60)
							  .attr("width", 210)
							  .attr("height", 150)
							  .style("fill", "transparent")
							  .style("stroke", "black")
							  .style("stroke-width", 1)
							  .style("opacity", 0.5)
						}
						
						var valuey =0; 
						var valuex=0;
						var pos=0;
					
						var t=svg.selectAll('.circletextVal')[0];
						var c=svg.selectAll('.circletop')[0];
						svg.selectAll('.ToptextVal').each(function(){
								valuey = this.getAttribute("y");
								valuex = this.getAttribute("x");
								j=0;
								t[pos].setAttribute("y",parseFloat(valuey) - 58);
								c[pos].setAttribute("cy",parseFloat(valuey) - 55);
								c[pos].setAttribute("cx",parseFloat(valuex) - 40);
								
								pos=++pos;
								
							});
							
							svg.selectAll('.axis').selectAll('tspan').each(function(){
								var valx = this.getAttribute("x");
								this.setAttribute("x",valx);
							});
						
						
					
						
						if (typeof callback!='undefined' && typeof(callback) === "function") {
						// execute the callback, passing parameters as necessary
							callback();
						}
	  
            }, 200);			
				
          };
		  
				
        });
      }}
}])
.directive('validPasswordC', function() {
  return {
    require: 'ngModel',
    scope: {
      reference: '=validPasswordC'
    },
    link: function(scope, elm, attrs, ctrl) {
      ctrl.$parsers.unshift(function(viewValue, $scope) {
        var noMatch = viewValue != scope.reference
        ctrl.$setValidity('noMatch', !noMatch);      
		ctrl.$validators.noMatch = (noMatch)?noMatch:undefined;
        return (noMatch)?noMatch:undefined;
      });
	 
      scope.$watch("reference", function(value) {		
        ctrl.$setValidity('noMatch', value === ctrl.$viewValue);
		//ctrl.$setValidators('noMatch',value === ctrl.$viewValue);		
      });
    }
  }
})
.directive('uniqueTitle', function($http,messages) {
  var toId;
  return {
    restrict: 'A',
    require: 'ngModel',
    link: function(scope, elem, attr, ctrl) { 
      //when the scope changes, check the email.
      scope.$watch(attr.ngModel, function(value) {
		  
		 var url=messages.serverLiveHost + messages.CheckCaseTitle+value; 
		  
		var caseId = scope.$eval(attr.caseId);
		var caseAction = scope.$eval(attr.caseAction);
		if(!isEmpty(caseAction) && caseAction == 'update'){
			url +='/'+caseId;
		}else{
			url +='/0';
		}
        // if there was a previous attempt, stop it.
        if(toId) clearTimeout(toId);
        // start a new attempt with a delay to keep it from
        // getting too "chatty".
		if(!isEmpty(value)){
			toId = setTimeout(function(){
			  // call to some API that returns { isValid: true } or { isValid: false }
			  $http.get(url).success(function(data) {
				  //set the validity of the field
				  // console.log(data);
				  ctrl.$setValidity('uniqueTitle', !(data.CaseTitleStatus));
			  });
			}, 200);
		}
      })
    }
  }
})
.directive('numFilter', ['numberFilter',
  function(numberFilter) {
    return {
      restrict: 'A',
      require: 'ngModel',
      link: function(scope, elm, attr, ngModel) {
        var decPlaces = attr.format || 2;
        function formatter(value) { 
            return numberFilter(value, decPlaces);
        }
        ngModel.$formatters.push(formatter);
      }
    }
  }
])
.directive('onlyDigits', function () {
	return {
	  require: 'ngModel',
	  restrict: 'A',
	  link: function (scope, element, attr, ctrl) {
	    function inputValue(val) {
	      if (val) {
	        var digits = val.replace(/[^0-9.]/g, '');

	        if (digits.split('.').length > 2) {
	          digits = digits.substring(0, digits.length - 1);
	        }

	        if (digits !== val) {
	          ctrl.$setViewValue(digits);
	          ctrl.$render();
	        }
	        return parseFloat(digits);
	      }
	      return undefined;
	    }            
	    ctrl.$parsers.push(inputValue);
	  }
	};
});
