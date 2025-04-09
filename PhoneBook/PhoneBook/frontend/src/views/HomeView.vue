<template>
    <div class="home">
        <img alt="Vue logo" src="../assets/logo.png">

        <table>
            <thead>
                <tr>
                    <th>№</th>
                    <th>Фамилия</th>
                    <th>Имя</th>
                    <th>Отчество</th>
                    <th>телефоны</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(contact, index) in contacts" :key="contact.id">
                    <td v-text="index + 1"></td>
                    <td v-text="contact.lastName"></td>
                    <td v-text="contact.firstName"></td>
                    <td v-text="contact.middleName"></td>
                    <td>
                        <div v-for="phoneNumbers in contact.phoneNumbers" :key="phoneNumbers.id">
                            {{phoneNumbers.phone}}
                        </div>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</template>

<script>
    export default {
        created() {
            this.$store.dispatch("loadContacts");
        },

        computed: {
            contacts() {
                return this.$store.state.contacts;
            }
        }
    };
</script>