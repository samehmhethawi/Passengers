
jQuery(document).ready(function ($) {
    $('[data-toggle="tooltip"]').tooltip();
	// -------------------------------------------------------------
	// Back to top button
	// -------------------------------------------------------------
  
  
	$(window).scroll(function() {
		if ($(this).scrollTop() > 100) {
			$('.back-to-top').fadeIn('slow');
		}
		else {
			$('.back-to-top').fadeOut('slow');
		}
	});
	
	$('.back-to-top').click(function(){
		$('html, body').animate({scrollTop : 0},1500, 'easeInOutExpo');
		return false;
	});
	
	
		// -------------------------------------------------------------
	// Initiate superfish on nav menu
	// -------------------------------------------------------------  
	

	$('.nav-menu').superfish({
		animation: {
			opacity: 'show'
		},
		speed: 400
	});


	// -------------------------------------------------------------
	// Header fixed on scroll
	// -------------------------------------------------------------

  
	$(window).scroll(function() {
		if ($(this).scrollTop() > 100) {
			$('#header').addClass('header-scrolled');
		} else {
			$('#header').removeClass('header-scrolled');
		}
	});

	if ($(window).scrollTop() > 100) {
		$('#header').addClass('header-scrolled');
	}
	

	// -------------------------------------------------------------
	// Initiate the wowjs animation library
	// -------------------------------------------------------------
	

	new WOW().init();


	// -------------------------------------------------------------
	// Smooth scroll for the menu and links with .scrollto classes
	// ------------------------------------------------------------- 


	$('.nav-menu a, #mobile-nav a, .scrollto').on('click', function() {
		if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
			var target = $(this.hash);
			if (target.length) {
				var top_space = 0;

				if ($('#header').length) {
					top_space = $('#header').outerHeight();

					if( ! $('#header').hasClass('header-fixed') ) {
						top_space = top_space - 20;
					}
				}

				$('html, body').animate({scrollTop: target.offset().top - top_space}, 1500, 'easeInOutExpo');

				if ($(this).parents('.nav-menu').length) {
					$('.nav-menu .menu-active').removeClass('menu-active');
					$(this).closest('li').addClass('menu-active');
				}

				if ($('body').hasClass('mobile-nav-active')) {
					$('body').removeClass('mobile-nav-active');
					$('#mobile-nav-toggle i').toggleClass('fa-times fa-bars');
					$('#mobile-body-overly').fadeOut();
				}
				return false;
			}
		}
	});



	
	
	// ------------------------------------------------------------- 
	// custom code
	// ------------------------------------------------------------- 


});
    
function validatePhoneNumber(phoneNumber) {
    return phoneNumber.match(/^((([+]{0,1}|[0]{2})(9639)[0-9]{8})|((09)[0-9]{8})|((9)[0-9]{8}))$/);
}

function checkPhoneNumberFormat(eventDom) {
    toastr.clear();
    var value = $(eventDom).val();
    if (value != "") {
        var rvalue = value.replace(/\s+/g, '');
        $(eventDom).val(rvalue);
        if (!validatePhoneNumber(rvalue)) {
            $(eventDom).removeClass("has-no-error");
            $(eventDom).addClass("has-error");
            toastr.error("صيغة الهاتف غير صحيحة");
        } else {
            $(eventDom).addClass("has-no-error");
            $(eventDom).removeClass("has-error");
        }
    }
}

function sendVerificationCode(phoneNumber, requestVerificationToken) {
    $(".verifyPhoneBtn").attr("disabled", true).addClass("disabled");
    showLoading();
    $.ajax({
        url: $("#global_SendVerificationCodeToContinueAddComplaint").val(),
        type: "POST",
        data: {
            to: phoneNumber,
            __RequestVerificationToken: requestVerificationToken,
        },
        success: function (response) {
            stopLoading();
            if (response && response.success && !response.failed) {
                toastr.clear();
                toastr.success("تم إرسال رمز التحقق إلى رقم الجوال المدخل، يرجى إدخال هذا الرمز للمتابعة.");
                $("button.save-btn").removeAttr("disabled").removeClass("disabled");
                $(".VerificationCodeArea").slideDown();
            } else {
                $(".verifyPhoneBtn").removeAttr("disabled").removeClass("disabled");
                toastr.clear();
                toastr.error("فشلت عملية إرسال رمز التحقق، يرجى إعادة المحاولة لاحقاً.");
                $("button.save-btn").attr("disabled", true).addClass("disabled");
            }
        },
        error: function () {
            stopLoading();
            $(".verifyPhoneBtn").removeAttr("disabled").removeClass("disabled");
        }
    });
}