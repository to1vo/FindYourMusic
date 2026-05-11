const form = document.querySelector("#describe-form");
const category_groups = document.querySelectorAll(".category-group");
const category_items = document.querySelectorAll(".category-item");
const category_item_containers = document.querySelectorAll(".category-item-container");
const categories_selected_text = document.querySelector("#categories-selected");
const selected_categories_list = [];
const MAXIMUM_CATEGORY_AMOUNT = 4;
let selected_group = 1;

const show_selected_group = () => {
	hide_selected_group();

	category_groups.forEach(group => {
		if (group.id == selected_group) {
			group.classList.add("selected");
			group.insertAdjacentHTML("beforeend", `<div class="category-group-selection-bar"></div>`)
		}
	});

	category_item_containers.forEach(container => {
		if (container.id == selected_group) {
			container.classList.add("selected");
		}
	});
}

const update_selected_text = () => {
	categories_selected_text.textContent = `${selected_categories_list.length}/${MAXIMUM_CATEGORY_AMOUNT}`;
}

const hide_selected_group = () => {
	const selected_container = document.querySelector(".category-item-container.selected");
	if (selected_container) {
		selected_container.classList.remove("selected");
	}

	const selected_group = document.querySelector(".category-group.selected");
	if (selected_group) {
		selected_group.classList.remove("selected");
		const selection_bar = document.querySelector(".category-group-selection-bar");
		selection_bar.remove();
	}
}

const select_category_group = (e) => {
	if (e.target != selected_group) {
		selected_group = e.target.id;
		show_selected_group();
	}
}

const select_category = (category) => {
	//check selected categories amount
	if (selected_categories_list.length == MAXIMUM_CATEGORY_AMOUNT) {
		const currently_selected_categories = document.querySelectorAll(".category-item.selected");
		for (const c of currently_selected_categories) {
			if (c.id === selected_categories_list[0].id) {
				deselect_category(c);
				break;
			}
		}
	}

	//add new category
	category.classList.add("selected");
	form.insertAdjacentHTML("afterbegin", `<input type="hidden" name="categories" value="${category.id}"/>`);
	selected_categories_list.push({ id: category.id, name: category.dataset.name });
}

const deselect_category = (category) => {
	selected_categories_list.splice(selected_categories_list.findIndex(c => c.id == category.id), 1);
	category.classList.remove("selected");
	const input = document.querySelector(`input[value="${category.id}"]`);
	input.remove();
}

const toggle_category = (e) => {
	const category = e.target;
	if (selected_categories_list.some(c => c.id == category.id) && category.classList.contains("selected")) {
		deselect_category(category);
	} else {
		select_category(category);
	}

	update_selected_text();
}

const set_previously_selected_categories_list = () => {
	const category_items = document.querySelectorAll(".category-item.selected");
	category_items.forEach(category => {
		select_category(category);
	});
	update_selected_text();
}


const handle_form_submit = (e) => {
	const inputs = document.querySelectorAll("input[name='categories']");
	if (selected_categories_list.length == 0 || selected_categories_list.length > MAXIMUM_CATEGORY_AMOUNT || inputs.length <= 0 || inputs.length > MAXIMUM_CATEGORY_AMOUNT) {
		e.preventDefault();
		//show message
		console.log("Nice try...");
	}
}

category_items.forEach(category => category.addEventListener("click", (e) => toggle_category(e)));
category_groups.forEach(group => group.addEventListener("click", (e) => select_category_group(e)));
form.addEventListener("submit", (e) => handle_form_submit(e));

set_previously_selected_categories_list();
show_selected_group();