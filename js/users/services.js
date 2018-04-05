'use strict';
/**
	User Services.
*/
angular.module('mhmApp.users')
.factory('UserService',
    ['Base64', '$http', '$cookieStore', '$rootScope', '$timeout','messages',
    function (Base64, $http, $cookieStore, $rootScope, $timeout,messages) {
        var service = {};
		var config = {
                headers : {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
		}
		
		/**
			Login User Services.
		*/
		service.Login = function (username, password, callback) {
			var data={Email: username, Password: password};
			var url = messages.serverLiveHost + messages.login;				
			//$http.get(messages.serverHost+'/sampleResponse/login.json')
			$http.post(url,$.param(data),config).success(function(response) {				
				callback(response);				
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});
		};
		
		/**
			Forgot Password User Services.
		*/
		service.ForgotPassword = function (username,callback) {          
			var data={Email: username};
			var url = messages.serverLiveHost + messages.getForgotPassword;					
			//$http.get(messages.serverHost+'/sampleResponse/login.json')
			$http.post(url,$.param(data),config)
			.success(function(response) {			
				callback(response);				
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});	;
        };
		
		/**
			Change Password User Services.
		*/
		service.ChangePassword = function (email,OldPassword,NewPassword,ConfirmPassword,callback) {          
			var data={Email:email,OldPassword:OldPassword,NewPassword:NewPassword,ConfirmPassword:ConfirmPassword};		
			var url = messages.serverLiveHost + messages.getChangePassword;				
			$http.post(url,$.param(data),config)
			.success(function(response) {				
				callback(response);				
			}).error(function(response){
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response);	
			});
        };
		
		/**
			Check Email Status User Services.
		*/
		service.CheckEmailStatus = function (id,callback) {          
			var data={Id:id};		
			var url = messages.serverLiveHost + messages.CheckEmailStatus+id;
			$http.get(url,config)
			.success(function(response) {
				callback(response);				
			}).error(function(response){	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response);	
			});
        };
		
		/**
			Set Password User Services.
		*/
		service.SetPassword = function (id,Password,ConfirmPassword,callback) {          
			var data={Id:id,Password:Password,ConfirmPassword:ConfirmPassword};		
			var url = messages.serverLiveHost + messages.SetPassword;		
			$http.post(url,$.param(data),config)
			.success(function(response) {				
				callback(response);				
			}).error(function(response){
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response);	
			});
        };
		
		/**
			Unlock User Services.
		*/
		service.UnlockUser = function (email,callback) {          
			var data={emailId:email};		
			var url = messages.serverLiveHost + messages.UnlockUser+email;					
			$http.get(url,config)
			.success(function(response) {
				callback(response);				
			}).error(function(response){
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response);	
			});
        };
		
		/**
			Update User Services.
		*/
		service.update = function (data,callback) {
           var url = messages.serverLiveHost + messages.updateUser;		   
		   $http.post(url,$.param(data),config).success(function(response) {					
				callback(response); 	
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});		  
        }; 
		
		/**
			Add User Services.
		*/
		service.add = function (data,callback) {
           var url = messages.serverLiveHost + messages.addUser;		   
		   $http.post(url,$.param(data),config).success(function(response) {				
				callback(response); 	
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});		  
        };
		
		/**
			Get All Users Services.
		*/
		service.getAll = function (data,callback) {
			
           var url = messages.serverLiveHost + messages.getUsers;
		   var filterBy="";
		   var filterText="";		  
		   var searchby="";	
		   var lstParameter=[];	
		   lstParameter[0]='';
		   lstParameter[1]='';
		   lstParameter[2]='';
		   lstParameter[3]='';
		   lstParameter[4]='';
			
		   if(typeof data.searchByName!='undefined' && data.searchByName!=''){				
				lstParameter[0]=data.searchByName;				
		   }if(typeof data.searchByEmail!='undefined' && data.searchByEmail!=''){				
				lstParameter[1]=data.searchByEmail;
		   }if(typeof data.searchByRole!='undefined' && data.searchByRole!=''){				
				lstParameter[2]=data.searchByRole;		  
		   }if(typeof data.searchByLockedStatus!='undefined' && data.searchByLockedStatus!=''){				
				lstParameter[4]=data.searchByLockedStatus;		   
		   }if(typeof data.searchByIsActive!='undefined' && data.searchByIsActive!=''){				
				lstParameter[3]=data.searchByIsActive;
		   }
		    if(typeof data.searchby!='undefined' && data.searchby!=''){			
				searchby=data.searchby;				
		   }  
		   
			var postData={searchby:searchby,sortby:data.sortby,desc:data.desc,page:data.page,pageSize:data.pageSize,lstParameter:lstParameter};
			console.log(postData);	
		    $http({method:'GET',url:url,params:postData}).success(function(response) {				
				callback(response); 	
			}).error(function(response) {					
				callback(response); 	
			});	;		  
        };
		
		/**
			Get User Services.
		*/
		service.getUser = function (id,callback) {
           var url = messages.serverLiveHost + messages.getUser+'/'+id;			
		   $http.get(url).success(function(response) {					
				callback(response); 	
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});		  
        };
		
		/**
			Get Client Companies Services.
		*/
		service.GetClientCompanies = function (callback) {
           var url = messages.serverLiveHost + messages.GetClientCompanies;			
		   $http.get(url).success(function(response) {					
				callback(response); 	
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});		  
        };
		
		/**
			Change User Status Services.
		*/		
		service.ChangeUserStatus = function (status,id,callback) {
            var url = messages.serverLiveHost + messages.ChangeUserStatus;	
			var data={UserId:id,Status:status};
		   $http.post(url,$.param(data),config).success(function(response) {					
				callback(response); 	
			}).error(function(response) {
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});		  
        }; 
		
		/**
			Remove User Services.
		*/
		service.remove = function (id,callback) {
            var url = messages.serverLiveHost + messages.deleteUser+'/'+id;	
			$http({method:'DELETE',url:url}).success(function(response) {			
				callback(response); 	
			}).error(function(response) {	
				if(isEmpty(response)){
					var response={Status:false,Message:messages.TryLater,redirect:true};
				}
				callback(response); 	
			});		  
        }; 
		
		/**
			Set Credentials Services.
		*/
        service.SetCredentials = function (username, password,customer,Token) {
            var authdata = Token;
            $rootScope.globals = {
                currentUser: {
                    username: username,
                    authdata: authdata,
					customer:customer
                }
            };
            $http.defaults.headers.common['Authorization'] = 'bearer ' + authdata; // jshint ignore:line
            $cookieStore.put('globals', $rootScope.globals);
        };
		
		/**
			Clear Credentials Services.
		*/
        service.ClearCredentials = function () {
            $rootScope.globals = {};
            $cookieStore.remove('globals');
            $http.defaults.headers.common.Authorization = 'bearer ';
        };

        return service;
    }])
/**
	Base64 Services.
*/
.factory('Base64', function () {
    /* jshint ignore:start */
    var keyStr = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=';

    return {
        encode: function (input) {
            var output = "";
            var chr1, chr2, chr3 = "";
            var enc1, enc2, enc3, enc4 = "";
            var i = 0;

            do {
                chr1 = input.charCodeAt(i++);
                chr2 = input.charCodeAt(i++);
                chr3 = input.charCodeAt(i++);

                enc1 = chr1 >> 2;
                enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
                enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
                enc4 = chr3 & 63;

                if (isNaN(chr2)) {
                    enc3 = enc4 = 64;
                } else if (isNaN(chr3)) {
                    enc4 = 64;
                }

                output = output +
                    keyStr.charAt(enc1) +
                    keyStr.charAt(enc2) +
                    keyStr.charAt(enc3) +
                    keyStr.charAt(enc4);
                chr1 = chr2 = chr3 = "";
                enc1 = enc2 = enc3 = enc4 = "";
            } while (i < input.length);

            return output;
        },

        decode: function (input) {
            var output = "";
            var chr1, chr2, chr3 = "";
            var enc1, enc2, enc3, enc4 = "";
            var i = 0;

            // remove all characters that are not A-Z, a-z, 0-9, +, /, or =
            var base64test = /[^A-Za-z0-9\+\/\=]/g;
            if (base64test.exec(input)) {
                window.alert("There were invalid base64 characters in the input text.\n" +
                    "Valid base64 characters are A-Z, a-z, 0-9, '+', '/',and '='\n" +
                    "Expect errors in decoding.");
            }
            input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");

            do {
                enc1 = keyStr.indexOf(input.charAt(i++));
                enc2 = keyStr.indexOf(input.charAt(i++));
                enc3 = keyStr.indexOf(input.charAt(i++));
                enc4 = keyStr.indexOf(input.charAt(i++));

                chr1 = (enc1 << 2) | (enc2 >> 4);
                chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                chr3 = ((enc3 & 3) << 6) | enc4;

                output = output + String.fromCharCode(chr1);

                if (enc3 != 64) {
                    output = output + String.fromCharCode(chr2);
                }
                if (enc4 != 64) {
                    output = output + String.fromCharCode(chr3);
                }

                chr1 = chr2 = chr3 = "";
                enc1 = enc2 = enc3 = enc4 = "";

            } while (i < input.length);

            return output;
        }
    };

    /* jshint ignore:end */
});