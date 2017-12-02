$(function() { 

    $('#suggestions').on('click', 'button', function() {
        var data = $(this).closest('[data-object]').data('object');
        var pick = $(this).data('pick');

        if (pick == 1)  {
            api.Post(data.typeA, data.countA, data.typeB, data.countB);
        }
        else  if (pick == 2)  {
            api.Post(data.typeB, data.countB, data.typeA, data.countA);
        }
    });

    api.init() 
});