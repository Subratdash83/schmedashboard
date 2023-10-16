$(document).ready(function(){
// Tool Tip
    $('[data-toggle="tooltip"]').tooltip();
// Tool Tip

// Setting close
$('.setting-close').on('click', function(){
  $('.setting-box').removeClass('on');
})
// Setting close

// Date picker
$('.datepicker').datepicker({
format: "dd-M-yyyy",
    todayBtn: "linked",
    autoclose: true,
    todayHighlight: true
});
// Date picker

// Navigation button click
$('.nav-toggle-btn').on('click',function(){
  
    $('.sidemenu .navbar-nav .nav-item.dropdown').removeClass('show');
$('.sidemenu .navbar-nav .dropdown-menu').removeClass('show');

  })
// Navigation button click

// Full width less than 800
var windowWidth=$(window).width();
if(windowWidth < 800 ){
$('.page-container').addClass('display-full');
}
// Full width less than 800


// Btn web effect

$(".btn").click(function (e) {
  

  $(".ripple").remove();

  var posX = $(this).offset().left,
      posY = $(this).offset().top,
      buttonWidth = $(this).width(),
      buttonHeight =  $(this).height();
  
  
  $(this).prepend("<span class='ripple'></span>");

  
 
  if(buttonWidth >= buttonHeight) {
    buttonHeight = buttonWidth;
  } else {
    buttonWidth = buttonHeight; 
  }
  
 
  var x = e.pageX - posX - buttonWidth / 2;
  var y = e.pageY - posY - buttonHeight / 2;
  
 
  
  $(".ripple").css({
    width: buttonWidth,
    height: buttonHeight,
    top: y + 'px',
    left: x + 'px'
  }).addClass("rippleEffect");
});

// Btn web effect

// view password btn
    $('.passwordshowbtn').on('click',function(){
         $(this).find('i').toggleClass("icon-eye1 icon-eye-off1");
              var inputpass = $('.password').attr("type");
            
              if (inputpass == "password") {
               $('.password').attr("type","text");
              } else {
                $('.password').attr("type", "password");
              }
      });

// view password btn


// Search panel
$('.searchopen').on('click',function(){
$('.search-sec').slideToggle('open');
$(this).find('i').toggleClass('icon-up-arrow icon-angle-arrow-down')
})
// Search panel

// Setting link
$('.setting-link').on('click', function(){
  $('.setting-box').addClass('on');
})
// Setting link

// Logout madal
$('.logout').click(function(){

    $('.logout-modal').addClass('show')
})


$('.nologout').click(function(){
    $('.logout-modal').removeClass('show') 
})
// Logout madal
// Logout madal

// left dark Menu with local storage
var lstoragedarkval = localStorage.getItem("sidemenudark");
if (lstoragedarkval !== "" ) {
    $( "#darkcheck" ).prop( "checked", true );
    $('.sidemenu').addClass(lstoragedarkval);
}

if (lstoragedarkval == null ) {
    $( "#darkcheck" ).prop( "checked", false );  
}



$('#darkcheck').click(function(){

   if($(this).prop("checked") == true){
             if (typeof (Storage) !== "undefined") {

              localStorage.setItem("sidemenudark", "dark");
             var lstoragedarkval = localStorage.getItem("sidemenudark");
             //alert(lstorageval);

              $('.sidemenu').addClass(lstoragedarkval);
              
               
              }

            }
            else if($(this).prop("checked") == false){
                localStorage.removeItem('sidemenudark');
                
               $('.sidemenu').removeClass('dark');
            }



});





// Top Menu with local storage
if(windowWidth>768){
var lstorageval = localStorage.getItem("horizontalmenu");

//alert(lstorageval);
if (lstorageval !== "" ) {
   

$( "#topmenucheck" ).prop( "checked", true );
$('.page-container').addClass(lstorageval);
$('.nav-toggle-btn').hide();

}

if (lstorageval == null ) {

 $( "#topmenucheck" ).prop( "checked", false );  
   $('.nav-toggle-btn').show();
}

$('#topmenucheck').click(function(){

    if($(this).prop("checked") == true){

        if (typeof (Storage) !== "undefined") {

              localStorage.setItem("horizontalmenu", "menu-top");
             var lstorageval = localStorage.getItem("horizontalmenu");
             //alert(lstorageval);

              $('.page-container').addClass(lstorageval);
              $('.nav-toggle-btn').hide();
               
              }
     }
            else if($(this).prop("checked") == false){

              localStorage.removeItem('horizontalmenu');
              
               $('.page-container').removeClass('menu-top');
               $('.nav-toggle-btn').show();
            }



});

}
else{
 
 localStorage.removeItem('horizontalmenu');
}

 // Top Menu with local storage    

 // Themeing

 var lstorageThemeval = localStorage.getItem("webtheme");

//alert(lstorageval);
if (lstorageThemeval !== "" ) {
   
$('.theme-list li a').removeClass('active');
 $('body').addClass(lstorageThemeval);
$('#'+lstorageThemeval).addClass('active');
}

if (lstorageThemeval == null ) {

  localStorage.removeItem('webtheme');

 
}


 $('.theme-list li a').on('click',function(){
      $('.theme-list li a').removeClass('active');
      $(this).addClass('active');
      $('body').attr('class','');
      var themeid = $(this).attr("id");

           if (typeof (Storage) !== "undefined") {

              localStorage.setItem("webtheme",themeid);
             var lstorageThemeval = localStorage.getItem("webtheme");
             //alert(lstorageThemeval);
             $('body').addClass(lstorageThemeval);


              
               
              }


    });

 $('#custom').on('click',function(){
   localStorage.removeItem('webtheme');
$('body').attr('class','');
 })


});

 $('.more-links').on('click', function(){
//alert('0');
  $(this).closest('.dashboard-portlet__header').toggleClass('active');

 });

 $('.maximize').on('click', function(){
//alert('0');
  $(this).closest('.dashboard-portlet').toggleClass('fixed');
  $(this).find("span").toggleClass('icon-maximize1 icon-minimize1')

 });

$('.filter').on('click', function(){
    //alert('0');
  $(this).closest('.dashboard-portlet').find('.filter-section').addClass('show');


 });

 $('.filter__close').on('click', function(){
//alert('0');
$(this).closest('.filter-section').removeClass('show');


 });

// Slim scroll funtion
  (function($){
    $(".notifications ul, .dashboard-portlet__content__details, .fixed-height").mCustomScrollbar({
      theme:"inset-2-dark"
    });

  })(jQuery);
// Slim scroll funtion
