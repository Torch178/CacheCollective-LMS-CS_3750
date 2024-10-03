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
function DrawTodo(todo) {
    var newTodoHTML = `
  <div class="col col-12 p-2 todo-item" todo-id="${todo.id}">
  <div class="input-group border border-primary object-fit-contain m-1">
  <div class="container w-75 ">
      <h6 class="text-wrap">${todo.course}</h6>
      <div class="text-wrap">${todo.assignment}</div>  
      <div class="text-wrap font-weight-bold">${todo.due_date}</div>
  </div>
  <div class="input-group-append">
    <button todo-id="${todo.id}" class="btn btn-outline-secondary bg-danger text-white float-right align-end" type="button" onclick="DeleteTodo(this);"
      id="button-addon2 ">X</button>
  </div>
  </div>
  </div>
  `;

    var dummy = document.createElement("DIV");
    dummy.innerHTML = newTodoHTML;
    document.getElementById("todo-container").appendChild(dummy.children[0]);

    /*
    //jQuery version
     var newTodo = $.parseHTML(newTodoHTML);
     $("#todo-container").append(newTodo);
    */

}

//Main function, Renders all To-Do Items in List
function RenderAllTodos() {

    var container = document.getElementById("todo-container");
    while (container.firstChild) {
        container.removeChild(container.firstChild);
    }
    /*
    //jQuery version
      $("todo-container").empty();
    */


    for (var i = 0; i < todos.length; i++) {
        DrawTodo(todos[i]);
    }
}

RenderAllTodos();

//Deletes To-Do List Item from List, triggered by list item delete button "X"
function DeleteTodo(button) {

    var deleteID = parseInt(button.getAttribute("todo-id"));
    /*
    //jQuery version
      var deleteID = parseInt($(button).attr("todo-id"));
    */

    for (let i = 0; i < todos.length; i++) {
        if (todos[i].id === deleteID) {
            todos[i].done = true;
            archive.push(todos[i]);
            todos.splice(i, 1);
            RenderAllTodos();
            break;
        }
    }
}

function TodoChecked(id) {
    todos[id].done = !todos[id].done;
    RenderAllTodos();
}

//---End of To-Do List Code----