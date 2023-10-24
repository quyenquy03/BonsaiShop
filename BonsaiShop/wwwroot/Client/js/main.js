	$(document).ready(function() {
		$(window).scroll(function () {
			if ($(this).scrollTop() >= 62) {
				$('.headerHolder').addClass('header-fixed');
			} else {
				$('.headerHolder').removeClass('header-fixed');
			}
		})
	})
