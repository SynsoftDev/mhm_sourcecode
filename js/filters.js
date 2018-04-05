'use strict';
/**
	Custom Filters.
*/
angular.module('mhmApp.filters', [])
.filter('interpolate', ['version', function(version) {
    return function(text) {
      return String(text).replace(/\%VERSION\%/mg, version);
    }
}])
.filter('unsafe', function($sce) { return $sce.trustAsHtml; })
.filter('percentage', ['$filter', function ($filter) {
  return function (input, decimals) {
    return $filter('number')(input, decimals) + '%';
  };
}])	
.filter('numberFixedLen', function () {
	return function (n, len) {
		var num = parseInt(n, 10);
		len = parseInt(len, 10);
		if (isNaN(num) || isNaN(len)) {
			return n;
		}
		num = ''+num;
		while (num.length < len) {
			num = '0'+num;
		}
		return num;
	};
})
.filter('stringFixedLen', function () {
	return function (value, len) {		
		if(value.length > len){
			value=value.substring(0, len);
			value=value+'...';
		}
		return value;
	};
})
.filter('inputcurrency', function () {
	 return function (inputcurrency) {	
		if (!inputcurrency) { return ''; }	
				var value = inputcurrency.toString().trim().replace(/^\+/, '');				
				while (value.charAt(0) == '0') {
                    value = value.substr(1);
                }
                value = value.replace(/[^\d.\',']/g, '');
                var point = value.indexOf(".");
                if (point >= 0) {
                    value = value.slice(0, point + 3);
                }

                var decimalSplit = value.split(".");
                var intPart = decimalSplit[0];
                var decPart = decimalSplit[1];				
                intPart = intPart.replace(/[^\d]/g, '');
                if (intPart.length > 3) {
                    var intDiv = Math.floor(intPart.length / 3);
                    while (intDiv > 0) {
                        var lastComma = intPart.indexOf(",");
                        if (lastComma < 0) {
                            lastComma = intPart.length;
                        }

                        if (lastComma - 3 > 0) {
							intPart=mutate(intPart)(lastComma - 3, 0, ",");							
						}
                        intDiv--;
                    }
                }

                if (decPart === undefined) {
                    decPart = "";
                }
                else {
                    decPart = "." + decPart;
                }
				return  intPart + decPart;

		}
})
.filter('mycurrency', [ '$filter', '$locale', function ($filter, $locale) {
	var currency = $filter('currency'), formats = $locale.NUMBER_FORMATS;
	return function (amount, symbol) {	
		var value = currency(amount, symbol);
		if(!isEmpty(value) && !isEmpty(amount)){			
			if(amount<0){
				value=Math.abs(value);
				var value = currency(Math.abs(amount), symbol);
				return "("+value.replace(new RegExp('\\' + formats.DECIMAL_SEP + '\\d{2}'), '')+")";
			}else{
				var value = currency(amount, symbol);				
				return value.replace(
				new RegExp('\\' + formats.DECIMAL_SEP + '\\d{2}'), '');
			}
			
		}else{
			var value = currency(amount, symbol);			
			return value;			
		}	
	} 
}])
.filter('tel', function () {
    return function (tel) {
        //console.log(tel);
        if (!tel) { return ''; }

        var value = tel.toString().trim().replace(/^\+/, '');

        if (value.match(/[^0-9]/)) {
            return tel;
        }

        var country, city, number;

        switch (value.length) {
            case 1:
            case 2:
            case 3:
                city = value;
                break;

            default:
                city = value.slice(0, 3);
                number = value.slice(3);
        }

        if(number){
            if(number.length>3){
                number = number.slice(0, 3) + '-' + number.slice(3,7);
            }
            else{
                number = number;
            }

            return ("(" + city + ") " + number).trim();
        }
        else{
            return "(" + city;
        }

    };
})
.filter('dateB', function () {
    return function (tel) {
        //console.log(tel);
        if (!tel) { return ''; }	
        var value = tel.toString().trim().replace(/^\+/, '');
		
        if (value.match(/[^0-9]/)) {
            return tel;
        }

        var country, city, number;
		 if(value.length>2 && value.length<=4){
                value = value.slice(0, 2) + '/' + value.slice(2,4);
		}else if(value.length>4){
                value = value.slice(0, 2) + '/' + value.slice(2,4) + '/' + value.slice(4,8);
		}else{
			value = value;
		}		
		//value = value.slice(0, 2) + '/' + value.slice(2,4)+ '/' + value.slice(4,8);

		return (value).trim();
        

    };
})
.filter('ageFilter', function() {
     function calculateAge(birthday) { // birthday is a date
         var ageDifMs = Date.now() - birthday.getTime();
         var ageDate = new Date(ageDifMs); // miliseconds from epoch
         return Math.abs(ageDate.getUTCFullYear() - 1970);
     }

     return function(birthdate) { 			
		   if(typeof birthdate!='undefined' && birthdate!='')
           return calculateAge(birthdate);
     }; 
});
