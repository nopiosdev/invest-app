
/* drawer query start */
$(document).on("click",".drawerMenu",function(){
  var that = $(this);
  var drawerid = that.data("drawerid");
  $("#"+drawerid).toggleClass("open");
  that.toggleClass("open");
});


/* tabs-navigation js start */
$(document).on("click",".tabs-navigation li a",function(){
  var that = $(this);
  var tab_id = that.attr("data-id");
  
  $(".tabs-content .tab, .tabs-navigation li").removeClass("active");
  $("#"+tab_id).addClass("active");

  that.parents("li").addClass("active");
});



/* account-dropdown js start */
$(document).on("click",".account-dropdown .account-wrapper",function(e){
  e.stopPropagation();
  var that = $(this);
      that.parents(".account-dropdown").toggleClass("open");
});
/* .account-dropdown .account-wrapper ul li a js start */
$(document).on("click",".account-dropdown.open ul li a",function(){
  var that = $(this);
      that.parents(".account-dropdown").addClass("open");
});

$(document).on("click","body",function(e){
  e.stopPropagation();
  $(".account-dropdown").removeClass("open");
});