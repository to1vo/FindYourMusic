const forms = document.querySelectorAll("#bookmark-form");
const bookmark_inputs = document.querySelectorAll("#bookmark-toggle-input");
const bookmark_buttons = document.querySelectorAll("#track-bookmark-button");

const handle_form_submit = async (e) => {
	e.preventDefault();

	const form_data = new FormData(e.target);

	try {
		const response = await fetch("/Track/Bookmark", {
			method: "post",
			body: form_data
		});

		if (response.ok) {
			const data = await response.json();
			e.srcElement[0].value = data.newState;
			show_bookmarked_state();
		} else {
			showErrorMessage();
			console.log("Failed to fetch");
		}
	} catch (err) {
		showErrorMessage();
		console.log(err);
	}
}

const removeMessages = () => {
	const messages = document.querySelectorAll(".message");
	messages.forEach(m => setTimeout(() => m.remove(), 3000));
}

const showErrorMessage = () => {
	const page_container = document.querySelector(".page-container");
	if (page_container) {
		page_container.insertAdjacentHTML("beforebegin", "<p class='text-error text-center message'>Failed to update bookmarking</p>");
	}
	removeMessages();
}

const show_bookmarked_state = () => {
	bookmark_buttons.forEach((button, index) => {
		if (bookmark_inputs[index].value === "1") {
			button.firstElementChild.src = "/bookmark-icon-selected.webp";
		} else {
			button.firstElementChild.src = "/bookmark-icon.webp";
		}
	});
}

if (forms) {
	forms.forEach(form => form.addEventListener("submit", (e) => handle_form_submit(e)));
	show_bookmarked_state();
}