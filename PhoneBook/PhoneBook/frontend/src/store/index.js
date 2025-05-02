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

        columnSortBy: "",
        isDescending: false,

        isLoading: false
    },

    mutations: {
        setIsLoading(state, value) {
            state.isLoading = value;
        },

        setTerm(state, value) {
            state.term = value;
        },

        setPageNumber(state, value) {
            state.pageNumber = value;
        },

        setSortingParameters(state, payload) {
            const { sortBy, isDesc } = payload;

            state.columnSortBy = sortBy;
            state.isDescending = isDesc;
        },

        setContacts(state, contacts) {
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
                    sortBy: state.columnSortBy,
                    isDescending: state.isDescending,
                    pageNumber: state.pageNumber,
                    pageSize: state.pageSize
                }
            }).then(response => {
                commit("setContacts", response.data.contactsDto);
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
                headers: { 'Content-Type': "application/json" },
                data: id
            })
                .then(() => {
                    commit("removeContactId", id);
                    dispatch("loadContacts");
                })
                .finally(() => {
                    commit("setIsLoading", false);
                });
        },

        deleteAllSelectedContacts({ commit, dispatch, state }) {
            commit("setIsLoading", true);

            return axios.delete("/api/PhoneBook/DeleteAllSelectedContact", {
                headers: { 'Content-Type': "application/json" },
                data: state.selectedContactsId
            })
                .then(() => {
                    commit("switchAllCheckbox", true);
                    dispatch("loadContacts");
                })
                .finally(() => {
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

        switchAllSelect({ commit, state }) {
            commit("switchAllCheckbox", state.isAllSelect);
        },

        selectContact({ commit }, id) {
            commit("addContactId", id);
        },

        deselectContact({ commit }, id) {
            commit("removeContactId", id);
        },

        sortByColumn({ commit, dispatch }, { sortBy, isDesc }) {
            commit("setSortingParameters", { sortBy, isDesc });
            dispatch("loadContacts");
        },

        searchContacts({ commit, dispatch }, term) {
            commit("setTerm", term);
            commit("switchAllCheckbox", true);

            dispatch("loadContacts");
        },

        navigateToPage({ commit, dispatch }, nextPage) {
            commit("setPageNumber", nextPage);
            dispatch("loadContacts");
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

        pageSize(state) {
            return state.pageSize;
        }
    }
});