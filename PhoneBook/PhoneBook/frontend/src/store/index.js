import { createStore } from "vuex";
import axios from "axios";

export default createStore({
    state: {
        contacts: [],
        selectedItems: [],

        isAllChecked: false,
        isLoading: false
    },

    mutations: {
        setIsLoading(state, value) {
            state.isLoading = value;
        },

        setContacts(state, contacts) {
            state.contacts = contacts;
        },

        selectAllCheckbox(state) {
            if (state.isAllChecked) {
                state.isAllChecked = false;
            } else {
                state.isAllChecked = true;
            }

            state.contacts.forEach(c => c.isChecked = state.isAllChecked);

            if (state.isAllChecked) {
                state.selectedItems = state.contacts.map(c => c.id);
            } else {
                state.selectedItems = [];
            }
        },

        addContactId(state, id) {
            state.selectedItems.push(id);

            state.contacts.find(c => c.id === id).isChecked = true;
            state.isAllChecked = false;
        },

        removeContactId(state, id) {
            const index = state.selectedItems.indexOf(id);

            if (index >= 0) {
                state.selectedItems.splice(index, 1);
                state.contacts.find(c => c.id === id).isChecked = false;

                state.isAllChecked = false;
            }
        }
    },

    actions: {
        toggleAllSelect({ commit }) {
            commit("selectAllCheckbox");
        },

        selectContact({ commit }, id) {
            commit("addContactId", id);
        },

        deselectContact({ commit }, id) {
            commit("removeContactId", id);
        },

        loadContacts({ commit }) {
            commit("setIsLoading", true);

            return axios.get("/api/PhoneBook/GetContacts")
                .then(response => {
                    commit("setContacts", response.data);
                })
                .catch(() => {
                    alert("Не удалось загрузить контакты");
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        },

        createContact({ commit }, contact) {//TODO: Отсюда вернется подписчикам все равно успешный ответ! Нужно как то возвразщять ошибку!
            commit("setIsLoading", true);

            return axios.post("/api/PhoneBook/CreateContact", contact)
                .then(() => {
                    alert("Ok. Create");
                })
                .catch(() => {
                    alert("Не удалось создать контакт");
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        },

        deleteContact({ commit }, id) {
            commit("setIsLoading", true);

            return axios.delete("/api/PhoneBook/DeleteContact",
                {
                    headers: { 'Content-Type': "application/json" },
                    data: id
                })
                .then(() => {
                    alert("Контакт удален");
                })
                .catch(response => {
                    alert("Не удалось удалить" + response.message);

                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        },

        updateContat({ commit }, contact) {
            commit("setIsLoading", true);

            return axios.post("/api/PhoneBook/UpdateContact", contact)
                .then(() => {
                    alert("Контакт успешно изменен");
                })
                .catch(response => {
                    alert("Не удалось в изменит " + response.message);
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        }
    }
});