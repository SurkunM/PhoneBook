import { createStore } from "vuex";
import axios from "axios";

export default createStore({
    state: {
        contacts: [],
        term: "",

        contactsCount: 0,
        pageNumber: 1,
        pageSize: 10,

        selectedContactsId: [],
        isAllSelect: false,

        sortByColumn: "",
        isDescending: false,

        isLoading: false
    },

    mutations: {
        setIsLoading(state, value) {
            state.isLoading = value;
        },

        setSearchParameters(state, value) {
            state.term = value;
            state.pageNumber = 1;
        },

        setSortingParameters(state, payload) {
            const { sortBy, isDesc } = payload;

            state.sortByColumn = sortBy;
            state.isDescending = isDesc;
        },

        setContacts(state, contacts) {
            contacts.forEach((c, i) => {
                c.index = i + 1;
            });

            state.contacts = contacts;
        },

        setContactsCount(state, count) {
            state.contactsCount = count;
        },

        switchAllCheckbox(state, isSelect) {
            if (isSelect) {
                state.isAllSelect = false;
            } else {
                state.isAllSelect = true;
            }

            state.contacts.forEach(c => c.isChecked = state.isAllSelect);

            if (state.isAllSelect) {
                state.selectedContactsId = state.contacts.map(c => c.id);
            } else {
                state.selectedContactsId = [];
            }
        },

        setSelectedCheckbox(state) {
            state.contacts
                .filter(c => state.selectedContactsId.includes(c.id))
                .forEach(c => c.isChecked = true);
        },

        addContactId(state, id) {
            state.selectedContactsId.push(id);

            state.contacts.find(c => c.id === id).isChecked = true;
            state.isAllSelect = false;
        },

        removeContactId(state, id) {
            const index = state.selectedContactsId.indexOf(id);

            if (index >= 0) {
                state.selectedContactsId.splice(index, 1);
                state.contacts.find(c => c.id === id).isChecked = false;

                state.isAllSelect = false;
            }
        }
    },

    actions: {
        loadContacts({ commit, state }) {
            commit("setIsLoading", true);

            return axios.get("/api/PhoneBook/GetContacts", {
                params: {
                    term: state.term,
                    sortBy: state.sortByColumn,
                    isDescending: state.isDescending,
                    pageNumber: state.pageNumber,
                    pageSize: state.pageSize
                }
            }).then(response => {
                commit("setContacts", response.data.contacts);
                commit("setContactsCount", response.data.totalCount);

                commit("setSelectedCheckbox");
            }).finally(() => {
                commit("setIsLoading", false);
            })
        },

        createContact({ commit, dispatch }, contact) {
            commit("setIsLoading", true);

            return axios.post("/api/PhoneBook/CreateContact", contact)
                .then(() => {
                    dispatch("loadContacts");
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        },

        deleteContact({ commit, dispatch }, id) {
            commit("setIsLoading", true);

            return axios.delete("/api/PhoneBook/DeleteContact", {
                headers: { "Content-Type": "application/json" },
                data: id
            }).then(() => {
                commit("removeContactId", id);
                dispatch("loadContacts");
            }).finally(() => {
                commit("setIsLoading", false);
            });
        },

        deleteAllSelectedContacts({ commit, dispatch, state }) {
            commit("setIsLoading", true);

            return axios.delete("/api/PhoneBook/DeleteAllSelectedContact", {
                headers: { "Content-Type": "application/json" },
                data: state.selectedContactsId
            }).then(() => {
                commit("switchAllCheckbox", true);
                dispatch("loadContacts");
            }).finally(() => {
                commit("setIsLoading", false);
            });
        },

        updateContact({ commit, dispatch }, contact) {
            commit("setIsLoading", true);

            return axios.post("/api/PhoneBook/UpdateContact", contact)
                .then(() => {
                    dispatch("loadContacts");
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        },

        exportToExcel({ commit }) {
            commit("setIsLoading", true);

            return axios.get("/api/PhoneBook/ExportToExcel", { responseType: 'blob' })
                .then(response => {
                    const url = window.URL.createObjectURL(new Blob([response.data]));
                    const link = document.createElement('a');

                    link.href = url;
                    link.setAttribute('download', 'contacts.xlsx');
                    document.body.appendChild(link);

                    link.click();

                    setTimeout(() => {
                        document.body.removeChild(link);
                        window.URL.revokeObjectURL(url);
                    }, 0);
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        }
    },

    getters: {
        contacts(state) {
            return state.contacts;
        },

        contactsCount(state) {
            return state.contactsCount;
        },

        selectedCount(state) {
            return state.selectedContactsId.length;
        },

        hasSelected(state) {
            return state.selectedContactsId.length > 0;
        },

        isAllSelect(state) {
            return state.isAllSelect || (state.selectedContactsId.length === state.contacts.length && state.selectedContactsId.length > 0);
        },

        isLoading(state) {
            return state.isLoading;
        },

        pageSize(state) {
            return state.pageSize;
        }
    }
});