import { useAuth0 } from '@auth0/auth0-vue';

export default function RecipeService() {
  const auth0 = useAuth0();

  async function getMyRecipes() {
    const accessToken = await auth0.getAccessTokenSilently();
    try {
      const response = await fetch(
        'https://localhost:7000/management/recipes',
        {
          headers: {
            Authorization: `Bearer ${accessToken}`,
          },
        }
      );
      const data = await response.json();
      return data;
    } catch (e) {
      return [];
    }
  }

  return {
    getMyRecipes,
  };
}
