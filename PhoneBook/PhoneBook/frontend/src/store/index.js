import { createStore } from "vuex";
import axios from "axios";

export default createStore({
    state: {
        isLoading: false,
        contacts: []
    },

    mutations: {
        setIsLoading(state, value) {
            state.isLoading = value;
        },

        setContacts(state, contacts) {
            state.contacts = contacts;
        },

        addContact(state, contact) {
            state.contacts.push(contact);
        }
    },

    actions: {
        loadContacts({ commit }) {
            commit("setIsLoading", true);

            return axios.get("/api/PhoneBook/GetContacts")
                .then(response => {
                    commit("setContacts", response.data);
                })
                .catch(() => {
                    alert("Не удалось загрузить контакты");
                })
                .then(() => {
                    commit("setIsLoading", false);
                });
        },

        createContact({ commit }, {contact}) {
            commit("setIsLoading", true);

            return axios.post("/api/PhoneBook/CreateContact", contact)
                .then(response => {
                    commit("addContact", response.data)
                })
                .catch(() => {
                    alert("Не удалось создать контакт");
                })
                .then(() => {
                    commit("setIsLoading", false);
                });
        }
    },

    modules: {

    }
});