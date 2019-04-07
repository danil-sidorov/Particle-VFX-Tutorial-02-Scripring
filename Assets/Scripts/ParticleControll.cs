using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleControll : MonoBehaviour
{
    public Transform targetTransform;
    public float attackDistance = 4.0f;
    public float force = 10.0f;
    private ParticleSystem particleSystemComponent;
    private ParticleSystem.MainModule mainModule;
    private ParticleSystem.EmissionModule emissionModule;
    private bool currentAttackState;
    private bool previousAttackState;

    void Start()
    {
        // Сохраняем ссылку на компонент
        particleSystemComponent = GetComponent<ParticleSystem>();
        // Сохраняем ссылки на модули доступ к каторым нам понадобиться
        mainModule = particleSystemComponent.main;
        emissionModule = particleSystemComponent.emission;
        // Инициализируем состояние для того, чтобы логика на первом LateUpdate() корректно отработала
        currentAttackState = Vector3.Distance(targetTransform.position, transform.position) <= attackDistance;
    }

    void LateUpdate()
    {
        // Сохраняем предыдущее состояние, логичнее поместить эту стоку в конце функции. Но я оставлю ее здесь
        previousAttackState = currentAttackState;
        // Получаем новое состояние, находиться ли цель на расстоянии атаки
        currentAttackState = Vector3.Distance(targetTransform.position, transform.position) <= attackDistance;

        // Проверяем изменилось ли состояние, используя исключающее OR. Если оба значения равны получаем False иначе True. 
        // https://msdn.microsoft.com/ru-ru/library/zkacc7k1.aspx
        if (currentAttackState ^ previousAttackState)
        {
            // Устанавливаем новые параметры в модулях Main и Emission, для текущего состояния.
            if (currentAttackState)
            {
                // Так как мы устанавливаем скорость движения частицы принудительно, а не воздействуем силой,
                // обнулять модификатор гравитации смысла нет. Оставим это здесь для примера.
                mainModule.gravityModifier = new ParticleSystem.MinMaxCurve(0.0f);
                // В режиме атаки количество частиц уменьшаем. В режиме покоя мы генерируем больше частиц, что бы улучшить визуализацию,
                // так как часть молний находиться за объектом или сразу же уходит в землю.
                emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(50.0f);
            }
            else
            {
                mainModule.gravityModifier = new ParticleSystem.MinMaxCurve(3.0f);
                emissionModule.rateOverTime = new ParticleSystem.MinMaxCurve(100.0f);
            }
        }

        // В режиме атаки направляем частицы к цели
        if (currentAttackState)
        {
            // Получить доступ к частице напрямую нельзя. Поэтому необходимо сначала сделать копию информации о частицах в массив с помощью GetParticles,
            // совершить требуемые нам операции, а затем отправить эти данные обратно SetParticles. 
            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystemComponent.particleCount];
            particleSystemComponent.GetParticles(particles);

            for (int i = 0; i < particles.Length; i++)
            {   
                // Обратите внимание! На первой вкладке Main настроек системы частиц в этой версии примера изменен параметр Simulation Space с Local на World.
                // Это избавляет нас от необходимости переводить координаты из локальных в мировые.
                Vector3 directionToTarget = (targetTransform.position - particles[i].position).normalized;
                particles[i].velocity = directionToTarget * force;
            }

            particleSystemComponent.SetParticles(particles, particles.Length);
        }
    }
}
