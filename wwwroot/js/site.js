// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//Code for Dashboard To-Do List

/*
//jQuery Version
$('#todo-input').on('input',function(e){
    currentTodo.text = e.target.value;
   });
*/



//Creates Individual To-Do List Items with Delete Button


//Main function, Renders all To-Do Items in List


//Deletes To-Do List Item from List, triggered by list item delete button "X"
function DeleteTodo(button) {
    /*
    //jQuery version
      var deleteID = $(button).attr("todo-id");
    */
    var deleteID = button.getAttribute("todo-id");
    var eDiv = document.getElementById(deleteID)
    eDiv.parentNode.removeChild(eDiv)


    //for (let i = 0; i < todos.length; i++) {
    //    if (todos[i].id === deleteID) {
    //        todos[i].done = true;
    //        archive.push(todos[i]);
    //        todos.splice(i, 1);
    //        RenderAllTodos();
    //        break;
    //    }
    //}
}

//To-do List Scroll Bar Function, Updates Scroll as items are added and removed from the list
var dataSpyList = [].slice.call(document.querySelectorAll('[data-bs-spy="scroll"]'))
dataSpyList.forEach(function (dataSpyEl) {
    bootstrap.ScrollSpy.getInstance(dataSpyEl)
        .refresh()
})

function TodoChecked(id) {
    todos[id].done = !todos[id].done;
    RenderAllTodos();
}

//---End of To-Do List Code----