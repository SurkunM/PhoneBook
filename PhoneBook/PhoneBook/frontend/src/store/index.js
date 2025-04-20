import { createStore } from "vuex";
import axios from "axios";

export default createStore({
    state: {
        contacts: [],

        selectedContacts: [],
        isLoading: false
    },

    mutations: {
        setIsLoading(state, value) {
            state.isLoading = value;
        },

        setContacts(state, contacts) {
            state.contacts = contacts;
        },

        allSelect(state, { contacts, select }) {
            if (select) {
                state.selectedContacts = contacts.map(contact => contact.id);
                state.contacts.forEach(c => c.isChecked = true);
            } else {
                state.selectedContacts = [];
                state.contacts.forEach(c => c.isChecked = false);
            }
        },

        select(state, contactId) {
            if (!state.selectedContacts.includes(contactId)) {
                state.selectedContacts.push(contactId);
            }
        },

        deselect(state, contactId) {
            const index = state.selectedContacts.indexOf(contactId)
            if (index > -1) {
                state.selectedContacts.splice(index, 1);
            }
        }
    },

    actions: {
        toggleAllSelect({ commit, state }, select) {
            commit("allSelect", {
                contacts: state.contacts,
                select: select ?? !(state.selectedContacts.length === state.contacts.length)
            });
        },

        selectContact({ commit }, contactId) {
            commit("select", contactId);
        },

        deselectContact({ commit }, contactId) {
            commit("deselect", contactId);
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