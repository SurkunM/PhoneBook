<template>
    <tr>
        <td>
            <v-checkbox v-model="isChecked" @change="switchCheckbox"></v-checkbox>
        </td>

        <td>{{ contact.index }}</td>

        <td>{{ contact.lastName }}</td>

        <td>{{ contact.firstName }}</td>

        <td>{{ contact.phone }}</td>

        <td>
            <v-btn small color="info" @click="$emit('edit', contact)" class="me-2">Редактировать</v-btn>
            <v-btn small color="error" @click="$emit('delete', contact)">Удалить</v-btn>
        </td>
    </tr>
</template>

<script>

    export default {
        props: {
            contact: {
                type: Object,
                required: true
            }
        },

        data() {
            return {
                isChecked: this.contact.isChecked
            };
        },

        watch: {
            'contact.isChecked'(value) {
                this.isChecked = value;
            }
        },

        methods: {
            switchCheckbox() {
                if (this.isChecked) {
                    this.$store.dispatch("selectContact", this.contact.id);
                } else {
                    this.$store.dispatch("deselectContact", this.contact.id);
                }
            }
        },

        emits: ["edit", "delete"]
    }
</script>